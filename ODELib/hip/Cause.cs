using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	/// <summary>
	/// A HiP-HOPS Cause object, i.e. the cause of an output deviation or hazard.
	/// Note that a HiP-HOPS Cause is different from an ODE Cause. In the ODE, a Cause is just a node in a 
	/// fault tree. In HiP-HOPS, a Cause is a possible cause of an output deviation or a hazard. In practice,
	/// only one Cause is ever used and the probability is ignored.
	/// </summary>
	public class Cause
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

		public Cause()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Probability { get; set; }

		[XmlElement(ElementName ="FailureLogic", DataType ="string")]
		public string FailureLogic { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
