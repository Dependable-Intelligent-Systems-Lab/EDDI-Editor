using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public enum CauseType
	{
		InputEvent,
		OutputEvent,
		BasicEvent,
		Gate,
		NormalEvent
	}

	/// <summary>
	/// A Cause is basically a fault tree node.
	/// </summary>
	/// <seealso cref="ODELib.ode.Base" />
	public class Cause : Base
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

		public Cause() { }
		public Cause(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public CauseType CauseType { get; set; }
		
		public Failure Failure { get; set; }
		
		public State State { get; set; }

		public Gate Parent { get; set; }

		[XmlArray]
		[XmlArrayItem(typeof(Action), ElementName = "Action")]
		[XmlArrayItem(typeof(MessageAction), ElementName = "MessageAction")]
		[XmlArrayItem(typeof(FunctionAction), ElementName = "FunctionAction")]
		[XmlArrayItem(typeof(WarningAction), ElementName = "WarningAction")]
		public List<Action> Actions { get; private set; } = new List<Action>();

		[XmlArray]
		[XmlArrayItem(typeof(Event), ElementName = "Event")]
		[XmlArrayItem(typeof(ExternalEvent), ElementName = "ExternalEvent")]
		[XmlArrayItem(typeof(ConditionEvent), ElementName = "ConditionEvent")]
		public List<Event> Evidence { get; private set; } = new List<Event>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
