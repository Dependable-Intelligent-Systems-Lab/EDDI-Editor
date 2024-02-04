using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
	public class EventVM
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

		public EventVM(ODELib.ode.Event @event)
		{
			this.OdeEvent = @event;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties
		private ODELib.ode.Event OdeEvent { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => string.IsNullOrWhiteSpace(OdeEvent.Name) ? "Unnamed Event" : OdeEvent.Name; set => OdeEvent.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Description")]
		[Description("Description")]
		public string Description { get => string.IsNullOrWhiteSpace(OdeEvent.Description) ? "" : OdeEvent.Description; set => OdeEvent.Description = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Monitors")]
		[Description("List of event monitors")]
		public ExpandableList Monitors { get; set; } = new ExpandableList("Monitors");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public static EventVM CreateAppropriateEventVM(ODELib.ode.Event @event)
		{
			if (@event is ConditionEvent conditionEvent) return new ConditionEventVM(conditionEvent);
			if (@event is ExternalEvent externalEvent) return new ExternalEventVM(externalEvent);
			return new EventVM(@event);
		}

		#endregion Functions
	}
}
