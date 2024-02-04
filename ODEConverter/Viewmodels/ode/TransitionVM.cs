using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
	[DisplayName("Transition")]
	public class TransitionVM
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

		public TransitionVM(ODELib.ode.Transition transition, StateMachineVM smVM)
		{
			OdeTransition = transition;

			//FromState = smVM.GetState(transition.FromState.Name);
			//ToState = smVM.GetState(transition.ToState.Name);
			foreach (var trigger in transition.Triggers)
			{
				if (trigger is ConditionEvent condition)
				{
					var eventVM = new ConditionEventVM(condition);
					Triggers.Add(eventVM);
				}
				else if (trigger is ExternalEvent external)
				{
					var eventVM = new ExternalEventVM(external);
					Triggers.Add(eventVM);
				}
				else
				{
					Triggers.Add(new EventVM(trigger));
				}
			}
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.Transition OdeTransition { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => OdeTransition.Name; set => OdeTransition.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		public string Trigger { get => OdeTransition.Trigger; set => OdeTransition.Trigger = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("FromState")]
		[Description("Source state")]
		public string FromState { get => OdeTransition.FromState.Name; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("ToState")]
		[Description("Destination state")]
		public string ToState { get => OdeTransition.ToState.Name; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Triggering Events")]
		[Description("Events that trigger this transition")]
		[ExpandableObject]
		public ExpandableList Triggers { get; private set; } = new ExpandableList("Triggers");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public void AddEvent(ODELib.ode.Event @event)
		{
			this.Triggers.Add(EventVM.CreateAppropriateEventVM(@event));

			OdeTransition.Triggers.Add(@event);
		}

		#endregion Functions

	}
}
