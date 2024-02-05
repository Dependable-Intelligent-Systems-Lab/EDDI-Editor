using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace SESAME_Sim;

/// <summary>
/// Represents a robot/actor in the world
/// </summary>
/// <seealso cref="Godot.Node2D" />
/// <seealso cref="SESAME_Sim.IEventListener" />
public partial class Robot : Node2D, IEventListener
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

	protected Sprite2D _body;
	protected Color _colour = new Color(1, 1, 1);
	protected float _flashTimer; // For the sin-wave flashing when selected/active etc
	protected bool _isHeadingOK;
	protected RichTextLabel _logNode;
	protected PathDisplay _pathDisplay;

	static Robot __lastFailed;

	#endregion Data

	/*****************************************************************************************************/
	/* Constructors
	/*****************************************************************************************************/
	#region Constructors

	public Robot()
	{
		EventProcessor.GetSingleton().RegisterListener(this);
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public Godot.Color Colour
	{
		get => _colour;
		set
		{
			_colour = value;
			_pathDisplay.DefaultColor = value * 0.75f;
		}
	}

	public List<Vector2> Destinations { get; private set; } = new List<Vector2>();
	public List<Vector2> OldDestinations { get; private set; } = new List<Vector2>(); // For when another drone needs to take over
	public FaultTree FaultTree { get; set; } // The fault tree for this robot, if applicable
	public bool IsActive { get; set; } = true;
	public bool IsSelected { get; set; } = false;
	public string RobotName { get; set; }
	public bool ShowPath { get; set; } = true;
	public float Speed { get; set; }
	public StateMachine StateMachine { get; set; } // The state machine for this robot, if applicable

	// Pseudo-constants
	public float Acceleration { get; set; } = 25; // Units/s^2
	public float MaxSpeed { get; set; } = 50; // Units/s
	public float TurnSpeed { get; set; } = 90; // Deg/s

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public override void _Ready()
	{
		base._Ready();
	}

	//----------------------------------------------------------------------------------------------------//

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (IsActive)
		{
			// Handle moving to destination
			if (Destinations.Count > 0)
			{
				MoveTo(Destinations.First(), delta);
			}

			// Actually move
			if (Speed != 0)
			{
				var movement = new Vector2(0, -Speed * (float)delta);
				movement     = movement.Rotated(Godot.Mathf.DegToRad(RotationDegrees));
				Position    += movement;
			}
		}

		// Modulate (flash slowly) if selected
		if (IsSelected)
		{
			_flashTimer += (float)delta;
			if (_flashTimer > 2)
			{
				_flashTimer = 0;
			}
			float modulate = (float)Math.Cos(_flashTimer * 2 * Math.PI) * 0.25f + 0.75f;
			Modulate = new Color(modulate * Colour.R, modulate * Colour.G, modulate * Colour.B);
		}
		else
		{
			Modulate = Colour;
		}

		// Path display
		if (ShowPath && Destinations.Count > 0 && Visible)
		{
			_pathDisplay.Visible = true;

			// Set points
			_pathDisplay.ClearPoints();
			_pathDisplay.AddPoint(Position);
			foreach (var destination in Destinations)
			{
				_pathDisplay.AddPoint(destination);
			}
		}
		else 
		{
			_pathDisplay.Visible = false;
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Adds a new destination to the queue.
	/// </summary>
	/// <param name="destination">The destination.</param>
	public void AddDestination(Vector2 destination)
	{
		Destinations.Add(destination);
	}

	//----------------------------------------------------------------------------------------------------//

	protected void LogMessage(string message) 
	{ 
		if (_logNode != null)
		{
			_logNode.Text += message + "\n";
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Moves the robot to a given destination (actual motion is in _Process; this just sets up speed + direction).
	/// </summary>
	/// <param name="destination">The destination.</param>
	/// <param name="delta">The delta.</param>
	public void MoveTo(Vector2 destination, double delta)
	{
		var diffVector        = (destination - Position);
		float distance        = diffVector.Length();
		float brakingDistance = (Speed / Acceleration) * Speed / 2;

		if (distance > 0)
		{
			// Rotate towards it first
			float angle = diffVector.Angle(); // This is relative to the x axis and is in radians, so we convert
			angle = Godot.Mathf.RadToDeg(angle) - 270;
			bool headingOK = RotateTo(angle, delta);

			// If we're pointing towards it
			if (headingOK) 
			{ 
				if (distance > brakingDistance)
				{
					// Accelerate to max
					Speed = Math.Min(MaxSpeed, Speed + Acceleration * (float)delta);
				}
				else if (distance <= brakingDistance && distance > 0.5f)
				{
					// Decelerate
					Speed = Math.Max(0, Speed - Acceleration * (float)delta);
				}
				else
				{
					// Arrivated
					Speed = 0;
					Position = Destinations.First();
					Destinations.RemoveAt(0);
					if (Visible)
					{
						LogMessage("[color=cyan]" + Name + " arrived at " + GD.VarToStr(Position) + "[/color]");
					}
				}
			}
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Rotates the robot towards a given angle. Returns true if pointing in the right direction, false otherwise.
	/// </summary>
	/// <param name="angle">The angle to rotate to.</param>
	/// <param name="delta">The delta.</param>
	/// <returns>Returns true if pointing in the right direction, false otherwise.</returns>
	public bool RotateTo(float angle, double delta)
	{
		// Calculate the "normalised" relative bearing difference (-180 to +180) between current and intended heading 
		float angleDiff = angle - RotationDegrees;
		while (angleDiff > 180)
		{
			angleDiff -= 360;
		}
		while (angleDiff < -180)
		{
			angleDiff += 360;
		}


		// Calculate the maximum turn; if we have to turn more than this, use the full amount
		// otherwise, just set to intended rotation
		float max_turn = (float)(TurnSpeed * delta);
		if (angleDiff > max_turn)
		{
			RotationDegrees += max_turn;
			return false;
		}
		else if (angleDiff < -max_turn)
		{
			RotationDegrees -= max_turn;
			return false;
		}
		else
		{ 
			RotationDegrees = angle;
			return true;
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Sets the current destination (and empties the queue of any others).
	/// </summary>
	/// <param name="destination">The destination.</param>
	public void SetDestination(Vector2 destination)
	{
		Destinations.Clear();
		Destinations.Add(destination);
	}

	//----------------------------------------------------------------------------------------------------//

	public void SetFaultTree(FaultTree ft)
	{
		FaultTree = ft;
		ft.Robot = this;
	}

	//----------------------------------------------------------------------------------------------------//

	public void SetStateMachine(StateMachine sm)
	{
		StateMachine = sm;
		sm.Robot = this;
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Set up the robot.
	/// </summary>
	/// <param name="viewport">The viewport.</param>
	/// <param name="name">The name.</param>
	/// <param name="bodyTexture">The body texture.</param>
	/// <param name="logNode">The log node.</param>
	public virtual void Setup(Viewport viewport, string name, Texture2D bodyTexture, RichTextLabel logNode=null)
	{
		Name = name;
		RobotName = name;

		// Create child nodes
		_body = new Sprite2D
		{
			Texture = bodyTexture
		};
		AddChild(_body);
		Scale = new Vector2(0.5f, 0.5f);

		// Create path
		_pathDisplay = new PathDisplay(this);

		_logNode = logNode;
		AddToGroup("robots");

		viewport.AddChild(this);
		viewport.AddChild(_pathDisplay);
	}

	//----------------------------------------------------------------------------------------------------//

	public void LoadFromODE(string filename)
	{
		// Load model from file
		ODELib.ode.Model model = null;
		using (var fileStream = File.Open(filename, FileMode.Open))
		using (var stringStream = new StreamReader(fileStream))
		{
			string xml = stringStream.ReadToEnd();
			var serializer = new XmlSerializer(typeof(ODELib.ode.Model));

			using (var memoryStream = new MemoryStream((new UTF8Encoding()).GetBytes(xml)))
			{
				model = (ODELib.ode.Model)serializer.Deserialize(memoryStream);
			}
		}

		if (model == null)
		{
			return;
		}

		// Grab the SM and FT
		var defaultSystem = model.SystemElements[0];
		var odeFaultTree = defaultSystem.FailureModels.Where(x => x.GetType() == typeof(ODELib.ode.FaultTree)).First() as ODELib.ode.FaultTree;
		var odeStateMachine = defaultSystem.FailureModels.Where(x => x.GetType() == typeof(ODELib.ode.StateMachine)).First() as ODELib.ode.StateMachine;

		// Create them
		var faultTree = FaultTree.BuildFromODE(odeFaultTree, this, _logNode);
		var stateMachine = StateMachine.BuildFromODE(odeStateMachine, this, _logNode);

		// Should be done
	}

	//----------------------------------------------------------------------------------------------------//


	#endregion Functions


	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static Robot CreateRobot(RobotInfo prototype, Viewport viewport, RichTextLabel log)
	{
		if (prototype.RobotType == RobotType.Drone)
		{
			var robot = new Drone()
			{
				Position = prototype.Position
			};
			var rotorPositions = new Vector2[] { new Vector2(-31.5f, -36.5f), new Vector2(31.5f, -36.5f), new Vector2(31.5f, 36.5f), new Vector2(-31.5f, 36.5f) };
			robot.Setup(viewport, prototype.Name, GD.Load<Texture2D>("res://textures/drone.png"), log, GD.Load<Texture2D>("res://textures/drone_prop.png"), rotorPositions);
			robot.Colour = prototype.Colour; // Has to be done after Setup since it uses the path display

			robot.LoadFromODE(prototype.ModelFile);

			return robot;
		}
		else
		{
			var robot = new Drone()
			{
				Position = prototype.Position,
			};
			robot.Setup(viewport, prototype.Name, GD.Load<Texture2D>("res://textures/drone.png"), log);
			robot.Colour = prototype.Colour; // Has to be done after Setup since it uses the path display

			robot.LoadFromODE(prototype.ModelFile);

			return robot;
		}
	}

	//----------------------------------------------------------------------------------------------------//

	#endregion Static Functions


	/*****************************************************************************************************/
	/* IEventListener
	/*****************************************************************************************************/
	#region IEventListener

	public string GetID()
	{
		return RobotName;
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
			LogMessage($"[color=cyan]{RobotName} is handling action [{a.ContentType} - {a.Content}][/color]");


			if (a.ActionType == Action.ActionTypeEnum.FUNCTION)
			{
				// Do something based on the type and content
				if (a.ContentType == "function")
				{
					if (a.Content.ToLower() == "rtb")
					{
						// Return to base instruction
						LogMessage($"[color=cyan]{RobotName} received instruction 'RTB' and is returning to base[/color]");

						var helipad = GetParent()?.GetNode<Sprite2D>("HomeBase");
						if (helipad != null) 
						{
							OldDestinations.AddRange(Destinations);
							SetDestination(helipad.Position);
						}

						// Issue drone failure event
						__lastFailed = this;
						EventProcessor.GetSingleton().ProcessAction(new Action(Action.ActionTypeEnum.WARNING, "event", $"{RobotName}: failed"));
					}
					
					else if (a.Content.ToLower().StartsWith("takeover route"))
					{
						var otherDrone = __lastFailed;
						if (a.Content.Contains(":"))
						{
							string otherDroneName = a.Content.Split(":")[1].Trim();
							otherDrone = GetParent().GetNode<Robot>(otherDroneName);
						}

						// Append another drone's route to ours
						LogMessage($"[color=cyan]{RobotName} received instruction 'Takeover Route' for drone '{otherDrone.RobotName}'[/color]");

						// Find other drone in the scene graph
						if (otherDrone != null)
						{
							// Now add its route to our own
							Destinations.Add(otherDrone.Position);

							foreach (var waypoint in otherDrone.OldDestinations)
							{
								Destinations.Add(waypoint);
							}
						}
					}

					else if (a.Content.ToLower() == "explode")
					{
						// KABOOM
						LogMessage($"[color=cyan][b]{RobotName} was destroyed[/b][/color]");

						Visible = false;
						var explosion = GetParent()?.GetNode<AnimatedSprite2D>("Explosion");
						explosion.Position = Position;
						explosion.Visible = true;
						explosion.Play();
						IsActive = false;
						OldDestinations.AddRange(Destinations);

						// Issue drone failure event
						__lastFailed = this;
						EventProcessor.GetSingleton().ProcessAction(new Action(Action.ActionTypeEnum.WARNING, "event", $"{RobotName}: failed"));
					}
				}
			}
		}
	}

	#endregion IEventListener

}
