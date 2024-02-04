using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	/// <summary>
	/// Top class of a HH results file.
	/// </summary>
	[XmlRoot(ElementName = "HiP-HOPS_Results")]
	public class HipResults
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

		[XmlIgnore]
		private List<FaultTree> _faultTrees = null;

		#endregion Data

		/*****************************************************************************************************/
		/* Constructors
		/*****************************************************************************************************/
		#region Constructors

		public HipResults()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		// Heterogeneous list
		// https://stackoverflow.com/questions/62884134/how-to-deserialize-child-elements-of-different-types-into-list-collection-of-bas
		[XmlArray("FaultTrees")]
		[XmlArrayItem(typeof(FaultTree), ElementName = "FaultTree")]
		[XmlArrayItem(typeof(FMEA), ElementName = "FMEA")]
		public List<HipResultBase> FTAResults { get; set; } = new List<HipResultBase>();

		[XmlIgnore]
		public FMEA FMEA
		{
			get
			{
				return (FMEA)FTAResults.Where(x => x.GetType() == typeof(FMEA)).FirstOrDefault();
			}
		}

		[XmlIgnore]
		public List<FaultTree> FaultTrees
		{
			get
			{
				// Lazy evaluation
				if (_faultTrees == null)
				{
					_faultTrees = new List<FaultTree>();
					foreach (var ft in FTAResults.Where(x => x.GetType() == typeof(FaultTree)))
					{
						_faultTrees.Add((FaultTree)ft);
					}
				}

				return _faultTrees;
			}
		}
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
		public void ResolveReferences()
		{
			foreach (var ft in FaultTrees)
			{
				ft.ResolveReferences(FMEA);
			}
		}

		#endregion Functions

	}
}
