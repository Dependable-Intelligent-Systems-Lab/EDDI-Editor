using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public class FailureModel : Base
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

		public FailureModel() { }
		public FailureModel(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlArray]
		public List<MinimalCutSets> MinimalCutSets { get; private set; } = new List<MinimalCutSets>();

		[XmlArray]
		public List<Failure> Failures { get; private set; } = new List<Failure>();

		[XmlArray]
		public List<FailureModel> SubModels { get; private set; } = new List<FailureModel>();

		// For DDI importing purposes
		[XmlIgnore]
		public List<System> ParentSystems { get; private set; } = new List<System>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
