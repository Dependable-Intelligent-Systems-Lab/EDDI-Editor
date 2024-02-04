using ODELib.hip;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
	public class GateVM : CauseVM
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

		public GateVM(Gate gate)
			: base(gate)
		{
			OdeGate = gate;

			foreach (var child in gate.Causes)
			{
				if (child is Gate childGate)
				{
					Causes.Add(new GateVM(childGate));
				}
				else
				{
					Causes.Add(new CauseVM(child));
				}
			}
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private Gate OdeGate { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Gate type")]
		[Description("The gate type (OR, AND etc)")]
		[ExpandableObject]
		public GateType GateType { get => OdeGate.GateType; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Causes")]
		[Description("Causes")]
		[ExpandableObject]
		public ExpandableList Causes { get; private set; } = new ExpandableList("Causes");

		//----------------------------------------------------------------------------------------------------//

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
