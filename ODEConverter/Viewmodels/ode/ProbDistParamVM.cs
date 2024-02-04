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
	[DisplayName("Probability Distribution Parameter")]
	public class ProbDistParamVM
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

		public ProbDistParamVM(ODELib.ode.ProbDistParam param)
		{
			OdeProbDistParam = param;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.ProbDistParam OdeProbDistParam { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name of parameter")] 
		public string Name { get => OdeProbDistParam.Name; set => OdeProbDistParam.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Value")]
		[Description("Value of parameter")]
		public string Value { get => OdeProbDistParam.Value; set => OdeProbDistParam.Value = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("NameAndValue")]
		[Description("Name and value of parameter")]
		public string NameAndValue { get => OdeProbDistParam.Name + " = " + OdeProbDistParam.Value; }

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
