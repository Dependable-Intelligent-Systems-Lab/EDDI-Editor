using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.dym
{
	[DisplayName("State")]
	public class StateVM
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

		public StateVM(ODELib.dym.State state)
		{
			DymState = state;

			IsFailState = state.IsFailState;
			IsStartState = state.IsStartState;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.dym.State DymState { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => DymState.Name; set => DymState.Name = value; }

		//----------------------------------------------------------------------------------------------------//


		[DisplayName("IsFailState")]
		[Description("Is Fail State?")]
		public bool IsFailState { get => DymState.IsFailState; set => DymState.IsFailState = value; }

		//----------------------------------------------------------------------------------------------------//


		[DisplayName("IsStartState")]
		[Description("Is Start State?")]
		public bool IsStartState { get => DymState.IsStartState; set => DymState.IsStartState = value; }

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
