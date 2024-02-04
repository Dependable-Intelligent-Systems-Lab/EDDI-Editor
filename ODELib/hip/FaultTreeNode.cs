using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	public class FaultTreeNode
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

		public FaultTreeNode()
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

		[XmlArray]
		[XmlArrayItem(typeof(OrGate), ElementName="Or")]
		[XmlArrayItem(typeof(AndGate), ElementName="And")]
		[XmlArrayItem(typeof(BasicEventRef), ElementName="Event")]
		public List<FaultTreeNode> Children { get; set; } = new List<FaultTreeNode>();

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
		public virtual void ResolveReferences(FMEA fmea)
		{
			foreach(var child in Children)
			{
				child.ResolveReferences(fmea);
			}
		}

		#endregion Functions

	}
}
