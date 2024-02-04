using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
	public class CauseVM
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

		public CauseVM(Cause cause)
		{
			OdeCause = cause;
			if (string.IsNullOrEmpty(Name)) Name = "Unnamed";

			if (cause.Failure != null)
			{
				Failure = new FailureVM(cause.Failure);
				Name = Failure.Name;
			}

			foreach (var action in cause.Actions)
			{
				Actions.Add(ActionVM.CreateAppropriateActionVM(action));
			}
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private Cause OdeCause { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => OdeCause.Name; set => OdeCause.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Failure Name")]
		[Description("Name of the baseic event (if applicable)")]
		public string FailureName { get => Failure?.Name ?? "N/A"; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Failure")]
		[Description("The corresponding failure (basic event), if applicable")]
		[ExpandableObject]
		public FailureVM Failure { get; set; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Type")]
		[Description("Type")]
		public CauseType CauseType { get => OdeCause.CauseType; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Actions")]
		[Description("Actions triggered by this Cause")]
		[ExpandableObject]
		public ExpandableList Actions { get; private set; } = new ExpandableList("Actions");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Adds a new Action to the cause.
		/// </summary>
		/// <param name="action">The action.</param>
		public void AddAction(ODELib.ode.Action action)
		{
			Actions.Add(ActionVM.CreateAppropriateActionVM(action));

			OdeCause.Actions.Add(action);
		}

		//----------------------------------------------------------------------------------------------------//

		#endregion Functions

	}
}
