using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public class State : Base
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

		public State() { }
		public State(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlAttribute("isInitialState")]
		public bool IsInitialState { get; set; }
		
		[XmlAttribute("isFailState")]
		public bool IsFailState { get; set; }
		
		public ProbDist InitialProb { get; set; }

		[XmlArray]
		public List<Failure> Failures { get; private set; } = new List<Failure>();

		[XmlArray]
		[XmlArrayItem(typeof(Action), ElementName = "Action")]
		[XmlArrayItem(typeof(MessageAction), ElementName = "MessageAction")]
		[XmlArrayItem(typeof(FunctionAction), ElementName = "FunctionAction")]
		[XmlArrayItem(typeof(WarningAction), ElementName = "WarningAction")]
		public List<Action> OnEntry { get; private set; } = new List<Action>();

		[XmlArray]
		[XmlArrayItem(typeof(Action), ElementName = "Action")]
		[XmlArrayItem(typeof(MessageAction), ElementName = "MessageAction")]
		[XmlArrayItem(typeof(FunctionAction), ElementName = "FunctionAction")]
		[XmlArrayItem(typeof(WarningAction), ElementName = "WarningAction")]
		public List<Action> OnExit { get; private set; } = new List<Action>();


		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
