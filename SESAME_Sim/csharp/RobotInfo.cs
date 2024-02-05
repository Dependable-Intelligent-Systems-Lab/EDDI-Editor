using Godot;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

public enum RobotType
{
	Robot,
	Drone
}

/// <summary>
/// Collection of info for setting up a robot
/// </summary>
/// <seealso cref="Godot.GodotObject" />
public partial class RobotInfo : GodotObject
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

	public RobotInfo()
	{
	}

	//----------------------------------------------------------------------------------------------------//

	public RobotInfo(JObject jrobot)
	{
		foreach (var item in jrobot)
		{
			switch (item.Key)
			{
				case "name": 
					Name = item.Value.ToString(); 
					break;
				case "type":
					if (item.Value.ToString().ToLower() == "drone")
					{
						RobotType = RobotType.Drone;
					}
					else
					{
						RobotType = RobotType.Robot;
					}
					break;
				case "position":
					var x = (float)((JArray)item.Value)[0];
					var y = (float)((JArray)item.Value)[1];
					Position = new Vector2(x, y);
					break;
				case "colour":
					var r = (float)((JArray)item.Value)[0];
					var g = (float)((JArray)item.Value)[1];
					var b = (float)((JArray)item.Value)[2];
					Colour = new Color(r, g, b);
					break;
				case "model":
					ModelFile = item.Value.ToString(); 
					break;
			}
		}
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public Godot.Color Colour { get; set; }
	public string ModelFile {  get; set; } // Containing fault tree, state machine etc
	public string Name {  get; set; }
	public Vector2 Position { get; set; }
	public RobotType RobotType { get; set; }

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions
	#endregion Functions

}
