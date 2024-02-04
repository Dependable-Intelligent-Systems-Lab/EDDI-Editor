using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
	public class ActionVM
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

		public ActionVM(ODELib.ode.Action action)
		{
			OdeAction = action;

		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.Action OdeAction { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => string.IsNullOrWhiteSpace(OdeAction.Name) ? "Unnamed Action" : OdeAction.Name; set => OdeAction.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Description")]
		[Description("Description")]
		public string Description { get => string.IsNullOrWhiteSpace(OdeAction.Description) ? "" : OdeAction.Description; set => OdeAction.Description = value; }

		//----------------------------------------------------------------------------------------------------//

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }


		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public static ActionVM CreateAppropriateActionVM(ODELib.ode.Action action)
		{
			if (action is MessageAction messageAction) return new MessageActionVM(messageAction);
			if (action is WarningAction warningAction) return new WarningActionVM(warningAction);
			if (action is FunctionAction functionAction) return new FunctionActionVM(functionAction);
			return new ActionVM(action);
		}

		#endregion Functions

	}
}
