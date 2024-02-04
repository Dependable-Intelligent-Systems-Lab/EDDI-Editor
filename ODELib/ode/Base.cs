using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	/// <summary>
	/// Represents the ODE::Base, which contains name/ID/description and key-value-maps for just about everything.
	/// </summary>
	public class Base
	{
		/*****************************************************************************************************/
		/* Enums/Constants
		/*****************************************************************************************************/
		#region Constants

		private static long __idCounter = 1;

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

		public Base() // For serialisation
		{

		}

		public Base(string name)
		{
			Name = name;
			ID = __idCounter++;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlElement(ElementName ="keyValueMaps")]
		//public Dictionary<string, Dictionary<string, string>> KeyValueMap { get; private set; } = new Dictionary<string, Dictionary<string, string>>();
		public SerialisableDictionary<string, SerialisableDictionary<string, string>> KeyValueMap { get; set; } = new SerialisableDictionary<string, SerialisableDictionary<string, string>>();
		
		[XmlAttribute(AttributeName ="Id")]
		public long ID { get; set; }

		[XmlAttribute(AttributeName ="name")]
		public string Name { get; set; }
		
		[XmlAttribute(AttributeName ="description")]
		public string Description { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
