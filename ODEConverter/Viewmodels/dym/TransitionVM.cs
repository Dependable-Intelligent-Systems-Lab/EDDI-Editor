using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODEConverter.Viewmodels.ode;
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

		public TransitionVM(ODELib.dym.Transition transition, StateMachineVM smVM)
		{
			DymTransition = transition;

			//SourceState = smVM.GetState(transition.SourceState.Name);
			//DestinationState = smVM.GetState(transition.DestinationState.Name);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.dym.Transition DymTransition { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => DymTransition.Name; set => DymTransition.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Trigger")]
		[Description("Triggering event")]
		public string Trigger { get => DymTransition.Trigger; set => DymTransition.Trigger = value; }

		//----------------------------------------------------------------------------------------------------//


		[DisplayName("FromState")]
		[Description("Source state")]
		public string SourceState { get => DymTransition.SourceState.Name; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("ToState")]
		[Description("Destination state")]
		public string DestinationState { get => DymTransition.DestinationState.Name; }

		//----------------------------------------------------------------------------------------------------//



		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
