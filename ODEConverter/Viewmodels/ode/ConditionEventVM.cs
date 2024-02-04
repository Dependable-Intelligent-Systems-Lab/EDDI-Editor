using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
	public class ConditionEventVM : EventVM
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

		public ConditionEventVM(ODELib.ode.ConditionEvent conditionEvent)
			: base(conditionEvent)
		{
			this.OdeConditionEvent = conditionEvent;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.ConditionEvent OdeConditionEvent { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Condition")]
		[Description("Condition")]
		public string Condition { get => OdeConditionEvent.Condition; set => OdeConditionEvent.Condition = value; }

		//----------------------------------------------------------------------------------------------------//

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
