using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

/// <summary>
/// A robot arm
/// </summary>
/// <seealso cref="SESAME_Sim.Robot" />
public partial class RobotArm : Robot
{
	/*****************************************************************************************************/
	/* Enums/Constants
	/*****************************************************************************************************/
	#region Constants

	public enum Phases
	{
		Grab,
		Turn,
		Release,
		TurnBack
	}

	#endregion Constants

	/*****************************************************************************************************/
	/* Data
	/*****************************************************************************************************/
	#region Data

	protected GpuParticles2D _particles;

	#endregion Data

	/*****************************************************************************************************/
	/* Constructors
	/*****************************************************************************************************/
	#region Constructors

	public RobotArm()
		: base()
	{
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	// Pseudo-constants
	public float Reach { get; set; } = 1; // 0.5-1.0
	public float Turn { get; set; } = 90; // Max turn
	public Phases Phase { get; set; } = Phases.Grab;

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public override void _Ready()
	{
		base._Ready();
	}

	//----------------------------------------------------------------------------------------------------//

	public override void _Process(double delta)
	{
		base._Process(delta);

		// Do grabbing animation
		if (IsActive)
		{
			if (Phase == Phases.Grab) 
			{ 
				if (Reach > 0.75f)
				{
					Reach -= 0.01f;
				}
				else
				{
					Phase = Phases.Turn;
				}
			}
			else if (Phase == Phases.Turn)
			{
				if (_body.RotationDegrees < Turn)
				{
					_body.RotationDegrees += 1.0f;
				}
				else
				{
					Phase = Phases.Release;
				}
			}
			else if (Phase == Phases.Release)
			{
				if (Reach < 1.0f)
				{
					Reach += 0.01f;
				}
				else
				{
					Phase = Phases.TurnBack;
				}
			}
			else if (Phase == Phases.TurnBack)
			{
				if (_body.RotationDegrees > 0)
				{
					_body.RotationDegrees -= 1.0f;
				}
				else
				{
					Phase = Phases.Grab;
				}
			}

			_body.Scale = new Vector2(1, Reach);
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Set up the drone.
	/// </summary>
	/// <param name="viewport">The viewport.</param>
	/// <param name="name">The name.</param>
	/// <param name="bodyTexture">The body texture.</param>
	/// <param name="logNode">The log node.</param>
	public override void Setup(Viewport viewport, string name, Texture2D bodyTexture, RichTextLabel logNode)
	{
		base.Setup(viewport, name, bodyTexture, logNode);

		_body.Offset = new Vector2(0, -37);
		Scale = new Vector2(0.75f, 0.75f);

		AddToGroup("robotarms");
	}

	#endregion Functions

}
