using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib.dym
{
	public class State
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

		public State(StateMachine sm, string name)
		{
			this.StateMachine = sm;
			Name = name;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Name { get; set; }

		public bool IsFailState { get; set; }

		public bool IsStartState { get; set; }

		public StateMachine StateMachine { get; private set; }

		public List<Transition> IncomingTransitions { get; private set; } = new List<Transition>();
		
		public List<Transition> OutgoingTransitions { get; private set; } = new List<Transition>();

		public float X { get; set; }

		public float Y { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
