using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODELib.ode;
using ODELib.dym;
using System.IO;
using System.Xml.Serialization;

namespace SESAME_Sim
{
	public partial class StateMachine : Node2D
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
		#endregion Data

		/*****************************************************************************************************/
		/* Constructors
		/*****************************************************************************************************/
		#region Constructors

		public StateMachine()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string FullName => Robot != null ? StateMachineName + " @ " + Robot.RobotName : StateMachineName;
		public Robot Robot { get; set; } // The robot the SM belongs to (if applicable; if null, assumed to be MRS/system-of-system level)
		public List<SMState> States { get; private set; } = new List<SMState>();
		public string StateMachineName { get; set; }
		public List<SMTransition> Transitions { get; private set; } = new List<SMTransition>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Adds the state to the SM.
		/// </summary>
		/// <param name="state">The state.</param>
		public void AddState(SMState state) 
		{
			if (!States.Contains(state))
			{
				States.Add(state);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Adds the transition to the SM.
		/// </summary>
		/// <param name="transition">The transition.</param>
		public void AddTransition(SMTransition transition)
		{
			if (!Transitions.Contains(transition))
			{
				Transitions.Add(transition);
			}
		}

		#endregion Functions

		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/
		#region Static Functions

		/// <summary>
		/// Builds an example state machine with 3 states.
		/// </summary>
		/// <param name="r">The target robot.</param>
		/// <param name="logNode">The log node.</param>
		/// <returns></returns>
		public static StateMachine BuildExampleStateMachine(Robot r=null, RichTextLabel logNode=null)
		{
			StateMachine stateMachine = new()
			{
				Name = "Example SM",
				StateMachineName = "Example SM"
			};

			var startState = new SMState();		startState.Setup(stateMachine,   "Operational", new Vector2(-400, -100), new Godot.Color(0.25f, 0.75f, 0.25f), logNode);
			var degradedState = new SMState();	degradedState.Setup(stateMachine, "Degraded", new Vector2(0, 150), new Godot.Color(0.75f, 0.75f, 0.25f), logNode);
			var failedState = new SMState();	failedState.Setup(stateMachine,   "Failed", new Vector2(400, -100), new Godot.Color(0.75f, 0.25f, 0.25f), logNode);

			startState.Activate();

			// Now transitions
			var t1 = new SMTransition(); t1.Setup(stateMachine, "degraded", startState, degradedState, logNode);
			var t2 = new SMTransition(); t2.Setup(stateMachine, "failed directly", startState, failedState, logNode);
			var t3 = new SMTransition(); t3.Setup(stateMachine, "failed", degradedState, failedState, logNode);

			// Events/actions
			string host = r?.RobotName ?? "";
			t1.TriggeringEvents.Add(new Event(host + ": Comms Failure occurred"));
			t1.TriggeringEvents.Add(new Event(host + ": Nav Failure occurred"));
			t2.TriggeringEvents.Add(new Event(host + ": TOP occurred"));
			t3.TriggeringEvents.Add(new Event(host + ": TOP occurred"));

			r?.SetStateMachine(stateMachine);

			degradedState.OnEntry.Add(new Action(Action.ActionTypeEnum.FUNCTION, "function", "rtb", r?.RobotName ?? "Drone 1"));
			failedState.OnEntry.Add(new Action(Action.ActionTypeEnum.FUNCTION, "function", "explode", r?.RobotName ?? "Drone 1"));

			return stateMachine;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Builds an example MRS state machine with 4 states for 2 drones.
		/// </summary>
		/// <param name="logNode">The log node.</param>
		/// <returns></returns>
		public static StateMachine BuildExampleMRSStateMachine(RichTextLabel logNode = null)
		{
			StateMachine stateMachine = new()
			{
				Name = "Example MRS SM",
				StateMachineName = "Example MRS SM"
			};

			var startState = new SMState(); startState.Setup(stateMachine, "2 Drones OK", new Vector2(-400, -100), new Godot.Color(0.25f, 0.75f, 0.25f), logNode);
			var degradedState1 = new SMState(); degradedState1.Setup(stateMachine, "Drone 1 failed", new Vector2(0, -350), new Godot.Color(0.75f, 0.75f, 0.25f), logNode);
			var degradedState2 = new SMState(); degradedState2.Setup(stateMachine, "Drone 2 failed", new Vector2(0, 150), new Godot.Color(0.75f, 0.75f, 0.25f), logNode);
			var failedState = new SMState(); failedState.Setup(stateMachine, "Failed", new Vector2(400, -100), new Godot.Color(0.75f, 0.25f, 0.25f), logNode);

			startState.Activate();

			// Now transitions
			var t1 = new SMTransition(); t1.Setup(stateMachine, "Drone 1 fails", startState, degradedState1, logNode);
			var t2 = new SMTransition(); t2.Setup(stateMachine, "Drone 2 fails", startState, degradedState2, logNode);
			var t3 = new SMTransition(); t3.Setup(stateMachine, "Drone 2 also fails", degradedState1, failedState, logNode);
			var t4 = new SMTransition(); t4.Setup(stateMachine, "Drone 1 also fails", degradedState2, failedState, logNode);

			// Events/actions
			t1.TriggeringEvents.Add(new Event("Drone 1: failed"));
			t2.TriggeringEvents.Add(new Event("Drone 2: failed"));
			t3.TriggeringEvents.Add(new Event("Drone 2: failed"));
			t4.TriggeringEvents.Add(new Event("Drone 1: failed"));

			degradedState1.OnEntry.Add(new Action(Action.ActionTypeEnum.FUNCTION, "function", "takeover route for: Drone 1", "Drone 2"));
			degradedState2.OnEntry.Add(new Action(Action.ActionTypeEnum.FUNCTION, "function", "takeover route for: Drone 2", "Drone 1"));

			return stateMachine;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Builds a state machine from an existing ODE model.
		/// </summary>
		/// <param name="odeStateMachine">The ODE state machine.</param>
		/// <param name="robot">The robot the state machine belongs to.</param>
		/// <param name="logNode">The log node.</param>
		/// <returns></returns>
		public static StateMachine BuildFromODE(ODELib.ode.StateMachine odeStateMachine, Robot robot = null, RichTextLabel logNode = null)
		{
			var stateMachine = new StateMachine()
			{
				Name = odeStateMachine.Name,
				StateMachineName = odeStateMachine.Name,
			};

			foreach (var odeState in odeStateMachine.States)
			{
				stateMachine.AddState(SMState.BuildFromODE(stateMachine, odeState, robot, logNode));
			}
			foreach (var odeTransition in odeStateMachine.Transitions)
			{
				var fromState = stateMachine.States.Where(x => x.StateName == odeTransition.FromState.Name).First();
				var toState = stateMachine.States.Where(x => x.StateName == odeTransition.ToState.Name).First();

				stateMachine.AddTransition(SMTransition.BuildFromODE(stateMachine, odeTransition, fromState, toState, robot, logNode));
			}

			robot?.SetStateMachine(stateMachine);

			return stateMachine;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Loads (and creates) a state machine from an ODE file.
		/// </summary>
		/// <param name="filename">The (XML) filename.</param>
		/// <param name="robot">The robot the state machine will belong to.</param>
		/// <param name="logNode">The log node for output.</param>
		/// <returns></returns>
		public static StateMachine LoadFromODE(string filename, Robot robot = null, RichTextLabel logNode = null)
		{
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
				return null;
			}

			// Grab the SM and FT
			var defaultSystem = model.SystemElements[0];
			var odeStateMachine = defaultSystem.FailureModels.Where(x => x.GetType() == typeof(ODELib.ode.StateMachine)).First() as ODELib.ode.StateMachine;

			// Create them
			var stateMachine = StateMachine.BuildFromODE(odeStateMachine, robot, logNode);

			// Should be done
			return stateMachine;
		}

		//----------------------------------------------------------------------------------------------------//

		#endregion Static Functions
	}
}
