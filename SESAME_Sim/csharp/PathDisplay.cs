using Godot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

/// <summary>
/// For displaying robot paths
/// </summary>
/// <seealso cref="Godot.Line2D" />
public partial class PathDisplay : Line2D
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

	protected Robot _robot; // The owner robot

	#endregion Data

	/*****************************************************************************************************/
	/* Constructors
	/*****************************************************************************************************/
	#region Constructors

	public PathDisplay(Robot robot)
	{
		_robot = robot;
		Name   = robot.Name + " path";
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public Robot ParentRobot => _robot;

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public override void _Ready()
	{
		base._Ready();
		Texture      = GD.Load<Texture2D>("res://textures/line.png");
		TextureMode  = Line2D.LineTextureMode.Stretch;
		JointMode    = Line2D.LineJointMode.Round;
		EndCapMode   = Line2D.LineCapMode.Round;
		DefaultColor = new Godot.Color(_robot.Colour);// * 0.75f;
		Width        = 3;
		Visible      = false;
	}

	#endregion Functions

}
