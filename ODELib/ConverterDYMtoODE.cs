using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib
{
	/// <summary>
	/// Converter for transforming Dymodia state machines into ODE state machines.
	/// </summary>
	/// <seealso cref="ODELib.Converter" />
	public class ConverterDYMtoODE : Converter
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

		public ConverterDYMtoODE()
			: base(ConverterType.DYMODIA_TO_ODE)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Converts a Dymodia state machine into an ODE model (other types of Dymodia models not currently handled)
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public ode.Model ConvertDYMtoODE(dym.Model model) 
		{
			if (model == null)
			{
				return null;
			}

			var outputModel = new ode.Model();

			// Create a new empty top-level system as container
			var outputSystem = new ode.System("Unnamed system");
			outputModel.SystemElements.Add(outputSystem);

			// Convert the state machine
			var sm = ConvertStateMachine(model.StateMachine);
			if (sm != null)
			{
				outputSystem.FailureModels.Add(sm);
			}

			return outputModel;
		}

		//----------------------------------------------------------------------------------------------------//

		public override ConverterModel Convert(ConverterModel inputModel)
		{
			return ConvertDYMtoODE(inputModel as dym.Model) as ode.Model;
		}

		#endregion Functions

		/*****************************************************************************************************/
		/* Conversion Functions
		/*****************************************************************************************************/
		#region Conversion Functions

		/// <summary>
		/// Converts the Dymodia state machine into an ODE one.
		/// </summary>
		/// <param name="dym_stateMachine">The dym state machine.</param>
		/// <returns></returns>
		private ode.StateMachine ConvertStateMachine(dym.StateMachine dym_stateMachine)
		{
			if (dym_stateMachine == null)
			{
				return null;
			}

			var ode_stateMachine = new ode.StateMachine(dym_stateMachine.Name);

			// Convert states first and collect into a dict
			var stateDict = new Dictionary<string, ode.State>();
			foreach (dym.State dym_state in dym_stateMachine.States) 
			{
				var ode_state = ConvertState(dym_state);
				ode_stateMachine.States.Add(ode_state);
				stateDict[ode_state.Name] = ode_state;
			}

			// Then transitions
			foreach (dym.Transition dym_transition in dym_stateMachine.Transitions)
			{
				var ode_transition = ConvertTransition(dym_transition, stateDict);
				ode_stateMachine.Transitions.Add(ode_transition);
			}

			return ode_stateMachine;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a Dymodia state into an ODE one.
		/// </summary>
		/// <param name="dym_state">State of the dym.</param>
		/// <returns></returns>
		private ode.State ConvertState(dym.State dym_state)
		{
			if (dym_state == null)
			{
				return null;
			}

			var ode_state            = new ode.State(dym_state.Name);
			ode_state.IsFailState    = dym_state.IsFailState;
			ode_state.IsInitialState = dym_state.IsStartState;

			// Copy the coords into the KVM (with a little adjustment so it's closer to 0,0)
			ode_state.KeyValueMap["Position"] = new SerialisableDictionary<string, string>();
			ode_state.KeyValueMap["Position"]["x"] = (dym_state.X - 500).ToString();
			ode_state.KeyValueMap["Position"]["y"] = (dym_state.Y - 500).ToString();

			return ode_state;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a Dymodia transition into an ODE one.
		/// </summary>
		/// <param name="dym_transition">The dym transition.</param>
		/// <param name="stateDictionary">The state dictionary.</param>
		/// <returns></returns>
		private ode.Transition ConvertTransition(dym.Transition dym_transition, Dictionary<string, ode.State> stateDictionary)
		{
			if (dym_transition == null)
			{
				return null;
			}

			var ode_transition = new ode.Transition(dym_transition.Name);

			ode_transition.Trigger   = dym_transition.Trigger;
			ode_transition.FromState = stateDictionary[dym_transition.SourceState.Name];
			ode_transition.ToState   = stateDictionary[dym_transition.DestinationState.Name];

			// Make triggering events from these
			if (!string.IsNullOrEmpty(ode_transition.Trigger))
			{
				string [] triggers = ode_transition.Trigger.Split(new char[] { ';' });
				foreach ( string trigger in triggers ) 
				{
					var conditionEvent = new ConditionEvent("Condition=" + trigger.Trim()) { Condition = trigger.Trim() };
					ode_transition.Triggers.Add(conditionEvent);
				}
			}
			
			return ode_transition;
		}

		//----------------------------------------------------------------------------------------------------//


		#endregion Conversion Functions
	}
}
