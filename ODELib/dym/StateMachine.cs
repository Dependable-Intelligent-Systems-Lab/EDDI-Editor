using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace ODELib.dym
{
	public class StateMachine
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

		public string Name { get; set; }

		public List<State> States { get; private set; } = new List<State>();
		public List<Transition> Transitions { get; private set; } = new List<Transition>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Deserialises a state machine from JSON.
		/// Unfortunately, for the purposes of the converter we have to handle all this JSON manually.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>The deserialised state machine.</returns>
		public static StateMachine Deserialise(JObject data)
		{
			var stateMachine = new StateMachine();
			var stateMachineData = (JObject)data["StateMachine"][0];

			stateMachine.Name = (string)stateMachineData["Name"];

			// We need two passes for this, since everything is cross-referenced by ID

			// Read States (initial pass)
			var stateDict = new Dictionary<string, State>();
			foreach (JObject stateData in (JArray)data["State"])
			{
				var state = new State(stateMachine, (string)stateData["Name"]);
				state.IsStartState = (bool)stateData["StartState"];
				state.IsFailState = (bool)stateData["FailState"];
				state.X = (float)stateData["X"];
				state.Y = (float)stateData["Y"];
				stateDict[(string)stateData["$id"]] = state;
				stateMachine.States.Add(state);
			}

			// And Transitions (initial pass)
			var transitionDict = new Dictionary<string, Transition>();
			foreach (JObject transitionData in (JArray)data["Transition"])
			{
				var transition = new Transition(stateMachine, (string)transitionData["Name"]);
				transition.Trigger = (string)transitionData["Trigger"];
				transitionDict[(string)transitionData["$id"]] = transition;
				stateMachine.Transitions.Add(transition);
			}

			// Now go through again and resolve references
			foreach(JObject transitionData in (JArray)data["Transition"])
			{
				var transition = transitionDict[(string)transitionData["$id"]];

				var sourceStateID = (string)transitionData["SourceState"]["$ref"];
				var destinationtateID = (string)transitionData["DestinationState"]["$ref"];

				// Lookup and assign
				var sourceState = stateDict[sourceStateID];
				var destinationState = stateDict[destinationtateID];
				transition.SourceState = sourceState; 
				transition.DestinationState = destinationState;
				sourceState.OutgoingTransitions.Add(transition);
				destinationState.IncomingTransitions.Add(transition);
			}

			return stateMachine;
		}

		#endregion Functions
	}
}
