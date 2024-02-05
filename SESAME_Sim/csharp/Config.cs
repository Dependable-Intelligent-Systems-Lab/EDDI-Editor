using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml.Serialization;

namespace SESAME_Sim;

/// <summary>
/// Handles setting up the simulation scenario from a config file
/// </summary>
/// <seealso cref="Godot.GodotObject" />
public partial class Config : GodotObject
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

	public Config()
	{
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public string MapName { get; set; }
	public string MRS_StateMachineName { get; set; }
	public Vector2 HomeBasePosition { get; set; }
	public List<RobotInfo> Robots { get; private set; } = new List<RobotInfo>();

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public Robot[] CreateRobots(Viewport viewport, RichTextLabel log)
	{
		Robot[] robots = new Robot[Robots.Count];
		for (int i = 0; i < Robots.Count(); i++)
		{
			robots[i] = Robot.CreateRobot(Robots[i], viewport, log);
		}
		return robots;
	}

	#endregion Functions

	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static Config LoadFromFile(string fileName)
	{
		var config = new Config();

		var json = JObject.Parse(File.ReadAllText(fileName));

		foreach (var item in json) 
		{ 
			switch (item.Key)
			{
				case "map": 
					config.MapName = item.Value.ToString(); 
					break;
				case "statemachine":
					config.MRS_StateMachineName = item.Value.ToString();
					break;
				case "homebase":
					var x = (float)((JArray)item.Value)[0];
					var y = (float)((JArray)item.Value)[1];
					config.HomeBasePosition = new Vector2(x, y);
					break;
				case "robots":
					foreach (var jrobot in item.Value)
					{
						var robotInfo = new RobotInfo(jrobot as JObject);
						config.Robots.Add(robotInfo);
					}
					break;
			}
		}

		return config;
	}

	#endregion Static Functions

}
