using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
	public class FaultTreeVM : FailureModelVM
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

		public FaultTreeVM(FaultTree faultTree)
			: base(faultTree)
		{
			OdeFaultTree = faultTree;

			if (faultTree.TopEvent is Gate childGate)
			{
				TopNode = new GateVM(childGate);
			}
			else
			{
				TopNode = new CauseVM(faultTree.TopEvent);
			}

			Items.Add(TopNode);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private FaultTree OdeFaultTree { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Top Node")]
		[Description("Top event of the fault tree")]
		[ExpandableObject]
		public CauseVM TopNode { get; set; }

		//----------------------------------------------------------------------------------------------------//

		[ExpandableObject]
		public ExpandableList Items { get; private set; } = new ExpandableList();

		//----------------------------------------------------------------------------------------------------//


		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
