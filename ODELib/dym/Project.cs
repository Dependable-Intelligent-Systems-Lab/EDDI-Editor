using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib.dym
{
	/// <summary>
	/// Dymodia Project. Encapsulates all types of models (like state machines).
	/// Currently unused, but there to support future expansion.
	/// </summary>
	public class Project
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

		public Project()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Name { get; set; }

		//public List<SystemModel> SystemModels { get; private set; } = new List<SystemModel>();
		//public List<FaultTree> FaultTrees { get; private set; } = new List<FaultTree>();

		public List<StateMachine> StateMachines { get; private set; } = new List<StateMachine>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions


		#endregion Functions
	}
}
