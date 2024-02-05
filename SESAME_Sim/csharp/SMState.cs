using Godot;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim
{
	public partial class SMState : Control, IEventListener
	{
		/*****************************************************************************************************/
		/* Enums/Constants
		/*****************************************************************************************************/
		#region Constants
		#endregion Constants

		/*****************************************************************************************************/
		/* Data
		/*****************************************************************************************************/
		#region Data

		protected Label _label; // For the name
		protected Polygon2D _shape; // of the node
		protected Line2D _outline; // of the node
		protected RichTextLabel _logNode;

		protected float _flashTimer; // For modulating when the state is active

		#endregion Data

		/*****************************************************************************************************/
		/* Constructors
		/*****************************************************************************************************/
		#region Constructors

		public SMState()
		{
			EventProcessor.GetSingleton().RegisterListener(this);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public bool IsActive { get; private set; } = false; // Is this the currently active state?

		public List<Action> OnEntry { get; private set; } = new List<Action>(); // Actions that are executed on entry
		
		public List<Action> OnExit { get; private set; } = new List<Action>(); // Actions that are executed on exit

		public List<SMTransition> IncomingTransitions { get; private set; } = new List<SMTransition>();
		
		public List<SMTransition> OutgoingTransitions { get; private set; } = new List<SMTransition>();

		public StateMachine ParentStateMachine { get; private set; }

		public string StateName { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public override void _Process(double delta)
		{
			base._Process(delta);

			if (IsActive)
			{
				_flashTimer += (float)delta;
				if (_flashTimer > 2)
				{
					_flashTimer = 0;
				}
				float col = (float)Math.Cos(_flashTimer * 2 * Math.PI) * 0.25f + 0.75f;
				_shape.Modulate = new Godot.Color(col * _shape.Color.R, col * _shape.Color.G, col * _shape.Color.B);
			}
			else
			{
				_shape.Modulate = _shape.Color;
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Activates this state.
		/// </summary>
		public void Activate()
		{
			IsActive = true;

			if (_logNode != null)
			{
				_logNode.Text += $"[color=magenta]State '{Name}' is now active in SM {ParentStateMachine.FullName}\n[/color]";
			}

			// Execute all onEntry actions
			foreach (var a in OnEntry)
			{
				EventProcessor.GetSingleton().ProcessAction(a);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Deactivates this state.
		/// </summary>
		public void Deactivate()
		{
			IsActive = false;

			// Execute all onExit actions
			foreach (var a in OnExit)
			{
				EventProcessor.GetSingleton().ProcessAction(a);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Gets the corners (for collision detection purposes).
		/// </summary>
		/// <returns></returns>
		public Vector2[] GetCorners()
		{
			var corners = new Vector2[4];
			corners[0] = _outline.Points[0] + Position;
			corners[1] = _outline.Points[1] + Position;
			corners[2] = _outline.Points[2] + Position;
			corners[3] = _outline.Points[3] + Position;
			return corners;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Set up the state for Godot.
		/// </summary>
		/// <param name="name">The name.</param>
		public void Setup(StateMachine parentStateMachine, string name, Vector2 position, Godot.Color colour, RichTextLabel logNode = null)
		{
			Name               = name;
			StateName          = name;
			Size               = new Vector2(200, 150);
			Position           = position;
			ParentStateMachine = parentStateMachine;
			ParentStateMachine.AddState(this);

			_logNode = logNode;

			_shape = new Polygon2D()
			{
				Color = colour,
				Polygon = new Vector2[]
				{
					new (-100,-75),
					new (100,-75),
					new (100,75),
					new (-100,75)
				}
			};

			_outline = new Line2D()
			{
				Texture     = GD.Load<Texture2D>("res://textures/line.png"),
				TextureMode = Line2D.LineTextureMode.Stretch,
				JointMode   = Line2D.LineJointMode.Round,
				EndCapMode  = Line2D.LineCapMode.Round,
				Closed      = true,
				Width       = 5,
				Points      = new Vector2[]
				{
					new (-100,-75),
					new (100,-75),
					new (100,75),
					new (-100,75)
				}
			};

			_label = new Label()
			{
				Text                = name,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment   = VerticalAlignment.Center,
				AutowrapMode        = TextServer.AutowrapMode.WordSmart,
				Size                = new Vector2(200, 150),
				Position            = new (-100, -75)
			};

			AddChild(_shape);
			AddChild(_outline);
			AddChild(_label);

			parentStateMachine.AddChild(this);
			AddToGroup("smstates");
		}

		#endregion Functions


		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/
		#region Static Functions

		public static SMState BuildFromODE(StateMachine stateMachine, ODELib.ode.State odeState, Robot robot = null, RichTextLabel logNode = null)
		{
			if (odeState is null)
			{
				throw new ArgumentNullException(nameof(odeState));
			}

			SMState state = new();
			float x, y;
			try
			{
				x = float.Parse(odeState.KeyValueMap["Position"]["x"]);
				y = float.Parse(odeState.KeyValueMap["Position"]["y"]);
			}
			catch
			{
				x = 0;
				y = 0;
			}
			var colour = new Color(0.75f, 0.75f, 0.25f);
			if (odeState.IsInitialState)
			{
				colour = new Color(0.25f, 0.75f, 0.25f);
				state.IsActive = true;
			}
			else if (odeState.IsFailState)
			{
				colour = new Color(0.75f, 0.25f, 0.25f);
			}
			state.Setup(stateMachine, odeState.Name, new Vector2(x,y), colour, logNode);

			// Events/Actions
			foreach (var odeAction in odeState.OnEntry)
			{
				state.OnEntry.Add(Action.BuildFromODE(odeAction, robot));
			}
			foreach (var odeAction in odeState.OnExit)
			{
				state.OnExit.Add(Action.BuildFromODE(odeAction, robot));
			}

			return state;
		}

		//----------------------------------------------------------------------------------------------------//


		#endregion Static Functions


		/*****************************************************************************************************/
		/* IEventListener
		/*****************************************************************************************************/
		#region IEventListener

		public string GetID()
		{
			return "SMS"+Name;
		}

		//----------------------------------------------------------------------------------------------------//

		public void HandleEvent(EventInstance e)
		{

		}

		//----------------------------------------------------------------------------------------------------//

		public void HandleAction(Action a)
		{
			if (a.Target == GetID())
			{
				// Do something
			}
		}

		#endregion IEventListener

	}
}
