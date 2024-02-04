using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public class FaultTree : FailureModel
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

		public FaultTree() { }
		public FaultTree(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		// XML serialisation doesn't always handle inheritance gracefully, so we need to tell the serialiser
		// what element to use depending on what concrete class is involved. It's a bit of a hack unfortunately.
		[XmlElement(typeof(Cause), ElementName = "Cause")]
		[XmlElement(typeof(Gate), ElementName = "Gate")]
		public Cause TopEvent { get; set; }

		/// <summary>
		/// Gets the list of causes.
		/// </summary>
		/// <value>
		/// The causes.
		/// </value>
		[XmlArray]
		[XmlArrayItem(typeof(Cause), ElementName = "Cause")]
		[XmlArrayItem(typeof(Gate), ElementName = "Gate")]
		public List<Cause> Causes { get; private set; } = new List<Cause>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
