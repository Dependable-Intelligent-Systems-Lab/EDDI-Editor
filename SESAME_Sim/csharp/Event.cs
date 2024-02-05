using Godot;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

public partial class Event : GodotObject
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

	public Event() // Godot constructor - call Setup in _Ready()
	{
	}

	//----------------------------------------------------------------------------------------------------//

	public Event(string condition="")
	{
		Setup(condition);
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public string Condition { get; set; } 

	public FTNode TriggersCause { get; set; } // This event will trigger this Cause (i.e., FTNode)

	public SMTransition TriggersTransition { get; set; } // This event will trigger this Transition

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public void Setup(string condition = "")
	{
		Condition = condition;
	}

	#endregion Functions


	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static Event BuildFromODE(ODELib.ode.Event odeEvent, Robot robot = null)
	{
		string robotName = robot != null ? robot.RobotName + ": " : "";
		Event @event = new Event();

		if (odeEvent is ConditionEvent condition)
		{
			@event.Setup(robotName + condition.Condition);
		}
		else
		{
			@event.Setup(robotName + odeEvent.Name);
		}

		return @event;
	}

	#endregion Static Functions
}