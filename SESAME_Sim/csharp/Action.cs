using Godot;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

public partial class Action : GodotObject
{
	/*****************************************************************************************************/
	/* Enums/Constants
	/*****************************************************************************************************/
	#region Constants

	public enum ActionTypeEnum
	{
		MESSAGE,
		WARNING,
		FUNCTION
	}

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

	public Action() // Godot constructor - call Setup in _Ready()
	{
	}

	//----------------------------------------------------------------------------------------------------//

	public Action(ActionTypeEnum type, string contentType = "", string content = "", string target = "")
	{
		Setup(type, contentType, content, target);
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public ActionTypeEnum ActionType { get; set; }
	
	public string Content { get; set; } // This is either the message, warning, or function, depending on type

	public string ContentType { get; set; }

	public string Target { get; set; } // Added for simulation purposes: this is who the Action is meant for

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public void Setup(ActionTypeEnum type, string contentType="", string content="", string target="")
	{
		ActionType  = type;
		Content     = content;
		ContentType = contentType;
		Target      = target;
	}

	#endregion Functions

	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static Action BuildFromODE(ODELib.ode.Action odeAction, Robot robot=null)
	{
		string robotName = robot != null ? robot.RobotName + ": " : "";
		Action action = null;
		switch (odeAction)
		{
			case MessageAction msg:
				action = new Action(Action.ActionTypeEnum.MESSAGE, msg.MessageType, robotName + msg.Message);
				break;
			case FunctionAction func:
				action = new Action(Action.ActionTypeEnum.FUNCTION, "function", func.Function, (robot?.RobotName) ?? "MRS");
				string function = func.Function;
				if (function.Contains("="))
				{
					string[] parts = function.Split("=");
					action.Target = parts[0].Trim();
					action.Content = parts[1].Trim();
				}
				break;
			case WarningAction warning:
				action = new Action(Action.ActionTypeEnum.WARNING, warning.WarningType, robotName + warning.Warning);
				break;
		}
		return action;
	}

	#endregion Static Functions


}
