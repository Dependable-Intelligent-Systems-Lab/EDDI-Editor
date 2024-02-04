using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public class Transition : Base
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

		public Transition() { }
		public Transition(string name= "")
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlAttribute("transition")]
		public double transition { get; set; } // This is a probability

		[XmlIgnore]
		public string Trigger { get; set; } // NB: not actually part of the ODE; used instead as a handy shortcut

		[XmlElement(ElementName = "fromState")]
		public State FromState { get; set; }

		[XmlElement(ElementName = "toState")]
		public State ToState { get; set; }

		[XmlArray(ElementName = "triggers")]
		[XmlArrayItem(typeof(Event), ElementName = "Event")]
		[XmlArrayItem(typeof(ExternalEvent), ElementName = "ExternalEvent")]
		[XmlArrayItem(typeof(ConditionEvent), ElementName = "ConditionEvent")]
		public List<Event> Triggers { get; private set; } = new List<Event>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
