using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public enum PortDirection
	{
		IN,
		OUT,
		BOTH
	}

	public class Port : Base
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

		public Port() { }
		public Port(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlAttribute("flowType")]
		public string FlowType { get; set; }

		[XmlAttribute("direction")]
		public PortDirection Direction { get; set; }

		[XmlArray]
		public List<Failure> InterfaceFailures { get; private set; } = new List<Failure>();

		[XmlArray]
		public List<Port> RefinedPorts { get; private set; } = new List<Port>();

		public AssuranceLevel AssuranceLevel { get; set; }

		//public List<DependabilityRequirement> DependabilityRequirements { get; private set; } = new List<DependabilityRequirement>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		#endregion Functions


		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/
		#region Static Functions


		#endregion Static Functions

	}
}
