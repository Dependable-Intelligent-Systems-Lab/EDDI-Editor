using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
	public class WarningActionVM : ActionVM
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

		public WarningActionVM(ODELib.ode.WarningAction action)
			: base(action)
		{
			OdeWarningAction = action;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.WarningAction OdeWarningAction { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Warning")]
		[Description("Warning")]
		public string Warning { get => OdeWarningAction.Warning; set => OdeWarningAction.Warning = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Warning Type")]
		[Description("Warning Type")]
		public string WarningType { get => OdeWarningAction.WarningType; set => OdeWarningAction.WarningType = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Warning Priority Level")]
		[Description("Warning Priority Level")]
		public int PriorityLevel { get => OdeWarningAction.PriorityLevel; set => OdeWarningAction.PriorityLevel = value; }

		//----------------------------------------------------------------------------------------------------//



		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
