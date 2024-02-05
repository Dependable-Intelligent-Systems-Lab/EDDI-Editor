using Godot;
using ODELib.ode;
using SESAME_Sim.csharp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

public partial class FTNode : Node2D, IEventListener
{
	/*****************************************************************************************************/
	/* Enums/Constants
	/*****************************************************************************************************/
	#region Constants

	public enum FTNodeType
	{
		NOP,	// intermediate/unknown node
		EVENT,	// basic event
		OR,		// OR gate
		AND,	// AND gate
		PAND	// PAND gate
	};

	//----------------------------------------------------------------------------------------------------//

	public enum ActiveFlag
	{
		NOT_ACTIVE		=0,		// This node has not occurred
		MAYBE_ACTIVE	=1,		// This node may have occurred (uncertainty)
		ACTIVE			=2,		// This node has occurred/is active
	}

	#endregion Constants

	/*****************************************************************************************************/
	/* Data
	/*****************************************************************************************************/
	#region Data

	protected ActiveFlag _activateFlag = ActiveFlag.NOT_ACTIVE;
	protected List<Line2D> _childLines = new List<Line2D>();
	protected List<int> _pandSequence = new List<int>(); // Sequence of events for PAND gates

	protected Panel _panel; // For the label
	protected Label _label; // For the name
	protected Polygon2D _shape; // of the node
	protected Line2D _outline; // of the node
	protected RichTextLabel _logNode; // For log output on screen

	#endregion Data

	/*****************************************************************************************************/
	/* Constructors
	/*****************************************************************************************************/
	#region Constructors

	public FTNode()
		: this("default", "nop")
	{
		EventProcessor.GetSingleton().RegisterListener(this);
	}

	//----------------------------------------------------------------------------------------------------//

	public FTNode(string name, string type)
	{
		Name     = name;
		NodeType = StringToFTNodeType(type);

		LayoutInformation = new FTLayoutInformation(this);
		EventProcessor.GetSingleton().RegisterListener(this);
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public ActiveFlag ActivateFlag
	{
		get => _activateFlag;
		set
		{
			if (value <= _activateFlag)
			{
				// No change if it's not an "increase" in flag status
				return;
			}
			_activateFlag = value;
			switch (value) 
			{ 
				case ActiveFlag.NOT_ACTIVE:
					_shape.Color = new Godot.Color(0.25f, 0.75f, 0.25f);
					break;
				case ActiveFlag.ACTIVE:
					_shape.Color = new Godot.Color(0.75f, 0.25f, 0.25f);
					break;
				case ActiveFlag.MAYBE_ACTIVE:
					_shape.Color = new Godot.Color(0.75f, 0.75f, 0.25f);
					break;
			}

			// Report to log
			if (_logNode != null)
			{
				_logNode.Text += $"[color=yellow]Node '{NodeName}' of fault tree '{ParentFaultTree.FullName}' is now {_activateFlag}[/color]\n";
			}

			// Trigger actions
			if (_activateFlag == ActiveFlag.ACTIVE)
			{
				foreach (var a in TriggeredActions)
				{
					EventProcessor.GetSingleton().ProcessAction(a);
				}
			}
		}
	}

	public List<FTNode> Children { get; private set; } = new();
	
	public bool IsActivated => ActivateFlag == ActiveFlag.ACTIVE;

	public bool IsActivatedMaybe => ActivateFlag == ActiveFlag.MAYBE_ACTIVE;

	public FTLayoutInformation LayoutInformation { get; private set; }

	public FTNode Parent { get; set; }

	public FaultTree ParentFaultTree { get; private set; }

	public string NodeName { get; set; }

	public FTNodeType NodeType { get; set; }

	public List<Event> TriggeringEvents { get; private set; } = new(); // Events that can trigger this node

	public List<Action> TriggeredActions { get; private set; } = new(); // Actions triggered when this node activates

	public float X
	{
		get => Position.X;
		set
		{
			Position = new Vector2(value, Position.Y);
		}
	}

	public float Y
	{
		get => Position.Y;
		set
		{
			Position = new Vector2(Position.X, value);
		}
	}


	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	/// <summary>
	/// Activates this node (sets it to true).
	/// </summary>
	public void Activate()
	{
		// If a node becomes true, then it feeds up into the parent until it hits an incomplete AND and
		// propagates down until it hits an OR
		// Also, we go bottom up

		if (NodeType != FTNodeType.OR)
		{
			foreach (var child in Children)
			{
				child.ActivateFromAbove(this);
			}
		}
		else
		{
			foreach (var child in Children)
			{
				child.ActivateMaybeFromAbove(this);
			}
		}

		ActivateFlag = ActiveFlag.ACTIVE;

		if (Parent != null)
		{
			Parent.ActivateFromBelow(this);
		}

		//if (SceneNode != null)
		//{
		//	SceneNode.Call("activate_callback", 1);
		//}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Activates from above.
	/// </summary>
	/// <param name="fromParent">From parent.</param>
	public void ActivateFromAbove(FTNode fromParent)
	{
		// When we activate from above, for most nodes we know all their children must also be activated.
		// The exception are OR gates, because we don't know which of their children were activated.
		// In this case, we set them to MAYBE.

		ActivateFlag = ActiveFlag.ACTIVE;

		if (NodeType == FTNodeType.OR)
		{
			foreach (var child in Children)
			{
				child.ActivateMaybeFromAbove(this);
			}
		}
		else
		{
			foreach (var child in Children)
			{
				child.ActivateFromAbove(this);
			}
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Activates from below.
	/// </summary>
	/// <param name="fromChild">From child.</param>
	public void ActivateFromBelow(FTNode fromChild)
	{
		// If we activate from below, the parent will become true as long as it isn't an AND or PAND gate.
		// If it is an AND gate, it only becomes true if all of the other children are also activated.
		// If it is a PAND gate, all children must occur in sequence from left to right.

		if (NodeType == FTNodeType.AND)
		{
			bool allActivated = true;
			foreach (var child in Children)
			{
				if (!child.IsActivated)
				{
					allActivated = false;
				}
			}
			if (allActivated)
			{
				ActivateFlag = ActiveFlag.ACTIVE;
			}
		}
		else if (NodeType == FTNodeType.PAND)
		{
			_pandSequence.Add(Children.IndexOf(fromChild));
			if (_pandSequence.Count == Children.Count)
			{
				for (int i = 0; i < _pandSequence.Count - 1; i++)
				{
					if (_pandSequence[i] >= _pandSequence[i + 1])
					{
						return; // Sequence violated
					}
				}
				ActivateFlag = ActiveFlag.ACTIVE;
			}
		}
		else
		{
			ActivateFlag = ActiveFlag.ACTIVE;
		}

		if (IsActivated)
		{
			if (Parent != null)
			{
				Parent.ActivateFromBelow(this);
			}
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Activates from above using the 'maybe' setting.
	/// </summary>
	/// <param name="fromParent">From parent.</param>
	public void ActivateMaybeFromAbove(FTNode fromParent)
	{
		// When we activate from above, for most nodes we know all their children must also be activated.
		// The exception are OR gates, because we don't know which of their children were activated.
		// In this case, we set them to MAYBE.

		ActivateFlag = ActiveFlag.MAYBE_ACTIVE;
		foreach (var child in Children)
		{
			child.ActivateMaybeFromAbove(this);
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Apply a fixed X coordinate offset to all descendants
	/// </summary>
	/// <param name="offset"></param>
	public virtual void ApplyOffsetX(float offset)
	{
		X += offset;
		foreach (var child in Children)
		{
			child.ApplyOffsetX(offset);
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Apply an X/Y offset to all descendants
	/// </summary>
	/// <param name="offsetX"></param>
	/// <param name="offsetY"></param>
	public virtual void ApplyOffset(float offsetX, float offsetY)
	{
		X += offsetX;
		Y += offsetY;
		foreach (var child in Children)
		{
			child.ApplyOffset(offsetX, offsetY);
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Automatically layout this tree according to the Walker tree layout algorithm (see <see cref="FTLayoutInformation"/> class)
	/// </summary>
	public void GenerateLayout()
	{
		LayoutInformation.GenerateLayout();
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Generates the lines joining this node to its children (recursively, so call from top node).
	/// </summary>
	/// <param name="viewport">The viewport.</param>
	public void GenerateLines()
	{
		// Remove any old lines
		foreach (var line in _childLines)
		{
			ParentFaultTree.RemoveChild(line);
		}
		_childLines.Clear();

		// Now generate and add new ones
		foreach (var child in Children)
		{
			var line = new FTLine();
			line.Join(this, child);
			ParentFaultTree.AddChild(line);
			_childLines.Add(line);

			child.GenerateLines();
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Resets the layout information (recursively).
	/// </summary>
	public void ResetLayoutInformation()
	{
		LayoutInformation.Reset();
		Children.ForEach(x => x.ResetLayoutInformation());
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Set up the specified node for Godot.
	/// </summary>
	/// <param name="parentFaultTree">The parent fault tree.</param>
	/// <param name="name">The name.</param>
	/// <param name="type">The node type.</param>
	/// <param name="parent">The parent.</param>
	public void Setup(FaultTree parentFaultTree, string name, string type, FTNode parent=null, RichTextLabel logNode=null)
	{
		Name            = name;
		NodeName        = name;
		NodeType        = StringToFTNodeType(type);
		ParentFaultTree = parentFaultTree;
		ParentFaultTree.AddNode(this);
		SetParent(parent);

		_logNode = logNode;

		_panel = new Panel()
		{
			CustomMinimumSize = new Vector2(100, 30),
			Size              = new Vector2(200, 30),
			Position          = new Vector2(-100, -30),
			AnchorBottom      = 0,
			AnchorLeft        = -100,
			AnchorTop         = -30,
			AnchorRight       = 100,
		};
		_panel.AddThemeStyleboxOverride("panel", new StyleBoxFlat() { BgColor = new Godot.Color(0.3f, 0.3f, 0.3f) });

		_label = new Label()
		{
			Text                = name,
			HorizontalAlignment = HorizontalAlignment.Center, 
			VerticalAlignment   = VerticalAlignment.Center,
			AutowrapMode        = TextServer.AutowrapMode.WordSmart
		};
		_label.SetAnchorsPreset(Control.LayoutPreset.FullRect, false);

		// Get points
		var points = GetLinePointsFor(this.NodeType);

		_shape = new Polygon2D()
		{
			Color   = new Godot.Color(0.25f, 0.75f, 0.25f),
			Polygon = points
		};

		_outline = new Line2D()
		{
			Texture     = GD.Load<Texture2D>("res://textures/line.png"),
			TextureMode = Line2D.LineTextureMode.Stretch,
			JointMode   = Line2D.LineJointMode.Round,
			EndCapMode  = Line2D.LineCapMode.Round,
			Closed      = true,
			Width       = 3,
			Points		= points
		};

		// If PAND, add extra points for the outline
		if (type == "pand")
		{
			_outline.AddPoint(new Vector2(0, 0));
			_outline.AddPoint(new Vector2(25, 75));
			_outline.AddPoint(new Vector2(-25, 75));
		}
	
		AddChild(_panel);
		_panel.AddChild(_label);
		AddChild(_shape);
		AddChild(_outline);

		parentFaultTree.AddChild(this);
		AddToGroup("ftnodes");
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Sets the parent.
	/// </summary>
	/// <param name="parent">The parent.</param>
	public void SetParent(FTNode parent)
	{
		if (Parent != null && Parent != parent)
		{
			// Remove from its children first
			Parent.Children.Remove(this);
		}

		// OK, add
		Parent = parent;
		if (parent != null)
		{
			parent.Children.Add(this);
		}
	}

	//----------------------------------------------------------------------------------------------------//

	#endregion Functions

	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static FTNode BuildFromODE(FaultTree faultTree, ODELib.ode.Cause node, FTNode parent=null, Robot robot = null, RichTextLabel logNode = null)
	{
		if (node is null)
		{
			throw new ArgumentNullException(nameof(node));
		}

		FTNode ftnode = new();
		ftnode.Setup(faultTree, node.Name, CauseTypeToFTNodeType(node).ToString(), parent, logNode);

		// Handle children
		if (node is Gate gate)
		{
			foreach (var child in gate.Causes)
			{
				var ftchild = BuildFromODE(faultTree, child, ftnode, robot, logNode);
			}
		}

		// Events/Actions
		foreach (var odeAction in node.Actions)
		{
			ftnode.TriggeredActions.Add(Action.BuildFromODE(odeAction, robot));
		}

		return ftnode;
	}

	//----------------------------------------------------------------------------------------------------//

	public static Vector2[] GetLinePointsFor(FTNodeType nodeType)
	{
		Vector2[] points = null;
		switch (nodeType) 
		{
			case FTNodeType.OR:
				points = new Vector2[]
				{
					new (0,0),
					new (10,5),
					new (20,15),
					new (25,25),
					new (25,75),
					new (20,65),
					new (10,55),
					new (0,50),
					new (-10,55),
					new (-20,65),
					new (-25,75),
					new (-25,25),
					new (-20,15),
					new (-10,5)
				};
				break;
			case FTNodeType.AND:
			case FTNodeType.PAND:
				points = new Vector2[]
				{
					new (0,0),
					new (10,5),
					new (20,15),
					new (25,25),
					new (25,75),
					new (-25,75),
					new (-25,25),
					new (-20,15),
					new (-10,5)
				};
				break;
			case FTNodeType.NOP:
				points = new Vector2[]
				{
					new (-40,0),
					new (40,0),
					new (40,80),
					new (-40,80)
				};
				break;
			case FTNodeType.EVENT:
				var temp   = new List<Vector2>();
				var up     = new Vector2(0, -40);
				var centre = new Vector2(0, 40);
				for (int a = 0; a < 360; a += 15)
				{
					var clockHand = up.Rotated(Godot.Mathf.DegToRad(a));
					temp.Add(new Vector2(clockHand.X, clockHand.Y) + centre);
				}
				points = temp.ToArray();
				break;
		}
		return points;
	}

	//----------------------------------------------------------------------------------------------------//

	public static FTNodeType StringToFTNodeType(string type)
	{
		switch (type.ToLower())
		{
			case "or": return FTNodeType.OR;
			case "and": return FTNodeType.AND;
			case "pand": return FTNodeType.PAND;
			case "event": return FTNodeType.EVENT;
			default: return FTNodeType.NOP;
		}
	}

	//----------------------------------------------------------------------------------------------------//

	public static FTNodeType CauseTypeToFTNodeType(ODELib.ode.Cause cause)
	{
		switch (cause.CauseType)
		{
			case ODELib.ode.CauseType.BasicEvent: return FTNodeType.EVENT;
			case ODELib.ode.CauseType.Gate:
				{
					switch (((Gate)cause).GateType)
					{
						case GateType.OR: return FTNodeType.OR;
						case GateType.POR: return FTNodeType.OR;
						case GateType.XOR: return FTNodeType.OR;
						case GateType.AND: return FTNodeType.AND;
						case GateType.PAND: return FTNodeType.PAND;
						case GateType.SAND: return FTNodeType.AND;
						default: return FTNodeType.NOP;
					}
				}
			default: return FTNodeType.NOP;
		}
	}
	#endregion Static Functions

	/*****************************************************************************************************/
	/* IEventListener
	/*****************************************************************************************************/
	#region IEventListener

	public string GetID()
	{
		return "FTN:"+Name;
	}

	//----------------------------------------------------------------------------------------------------//

	public void HandleEvent(EventInstance ei)
	{
		foreach (var e in TriggeringEvents)
		{
			if (e.Condition == ei.Condition)
			{
				Activate(); // The condition is fulfilled - activate the node
			}
		}
	}

	//----------------------------------------------------------------------------------------------------//

	public void HandleAction(Action a)
	{
		if (a.Target == GetID())
		{

		}
	}

	#endregion IEventListener

}
