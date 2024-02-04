using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	public class FaultTree : HipResultBase
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

		public FaultTree()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlAttribute]
		public string ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public int SIL { get; set; }
		public double Unavailability { get; set; }
		public double Frequency { get; set; }
		public double UnavailabilitySort { get; set; }
		public double Severity { get; set; }

		// XML Serialisation doesn't always handle inheritance hierarchies gracefully, so here we tell the serialiser
		// exactly which name to use for each class (otherwise they'd all be "FaultTreeNode"). It's ugly and a bit of
		// a hack but unfortunately necessary.
		[XmlArray]
		[XmlArrayItem(typeof(OrGate), ElementName = "Or")]
		[XmlArrayItem(typeof(AndGate), ElementName = "And")]
		[XmlArrayItem(typeof(BasicEventRef), ElementName = "Event")]
		public List<FaultTreeNode> TopNode { get; set; }

		[XmlArray]
		[XmlArrayItem(ElementName = "CutSets")]
		public List<CutSetsSummary> CutSetsSummary { get; set; } = new List<CutSetsSummary>();

		[XmlArray]
		[XmlArrayItem(typeof(CutSetList), ElementName = "CutSets")]
		public List<CutSetList> AllCutSets { get; set; } = new List<CutSetList>();

		[XmlIgnore]
		public Hazard Hazard { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Resolves the references between basic events in the fault tree (which are basically pointers) and the FMEA,
		/// where the actual data resides.
		/// </summary>
		/// <param name="fmea">The fmea.</param>
		public void ResolveReferences(FMEA fmea)
		{
			foreach (var cutsets in AllCutSets)
			{
				foreach (var cutset in cutsets.CutSets)
				{
					cutset.ResolveReferences(fmea);
				}
			}

			if (TopNode.Count > 0)
			{
				TopNode.First().ResolveReferences(fmea);
			}
		}

		#endregion Functions

	}
}
