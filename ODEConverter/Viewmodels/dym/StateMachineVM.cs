using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.dym;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.dym
{
	[DisplayName("State Machine")]
	public class StateMachineVM
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

		public StateMachineVM(ODELib.dym.StateMachine sm)
		{
			DymStateMachine = sm;

			// Must add states first so the transitions can look them up
			foreach (var state in sm.States)
			{
				States.Add(new StateVM(state));
			}
			foreach (var transition in sm.Transitions)
			{
				Transitions.Add(new TransitionVM(transition, this));
			}

			Items.Add(States);
			Items.Add(Transitions);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.dym.StateMachine DymStateMachine { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		public ObservableList Items { get; set; } = new ObservableList();

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => DymStateMachine.Name; set => DymStateMachine.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("States")]
		[Description("States")]
		[ExpandableObject]
		public ExpandableList States { get; private set; } = new ExpandableList("States");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Transitions")]
		[Description("Transitions")]
		[ExpandableObject]
		public ExpandableList Transitions { get; private set; } = new ExpandableList("Transitions");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }
		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public StateVM GetState(string name)
		{
			foreach (StateVM state in States)
			{
				if (state.Name == name)
				{
					return state;
				}
			}
			return null;
		}

		#endregion Functions

	}
}
