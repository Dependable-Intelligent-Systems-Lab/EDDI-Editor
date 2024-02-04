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
	[DisplayName("Probability Distribution")]
	public class ProbDistVM
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

		public ProbDistVM(ODELib.ode.ProbDist probDist)
		{
			OdeProbDist = probDist;

			foreach (var param in probDist.Parameters)
			{
				Parameters.Add(new ProbDistParamVM(param));
			}
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.ProbDist OdeProbDist { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name of distribution")]
		public string Name { get => OdeProbDist.Name; set => OdeProbDist.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Type")]
		[Description("Type of distribution")] 
		public string Type { get => OdeProbDist.Type; set => OdeProbDist.Type = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Parameters")]
		[Description("Distribution parameters")]
		[ExpandableObject]
		public ExpandableList Parameters { get; private set; } = new ExpandableList("Parameters");

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
