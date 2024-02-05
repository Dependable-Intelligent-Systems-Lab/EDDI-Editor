using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

/// <summary>
/// Drone class with nodes for propellers etc
/// </summary>
/// <seealso cref="SESAME_Sim.Robot" />
public partial class Drone : Robot
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

	protected List<Sprite2D> _rotors = new();

	#endregion Data

	/*****************************************************************************************************/
	/* Constructors
	/*****************************************************************************************************/
	#region Constructors

	public Drone()
		: base()
	{
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	// Pseudo-constants
	public float RotorSpeed { get; set; } = 2; // Full rotations/s

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

		// Make the rotors spin
		if (IsActive)
		{
			for (int i = 0; i < _rotors.Count; i++)
			{
				int dir = 1; // Clockise
				if (i % 2 == 0)
				{
					dir = -1; // Anti-clockwise
				}

				_rotors[i].RotationDegrees += dir * Math.Min(360, Math.Max(0, RotorSpeed * 360 * (float)delta));
			}
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
	/// <param name="rotorTexture">The rotor texture.</param>
	/// <param name="rotorPositions">The rotor positions.</param>
	public virtual void Setup(Viewport viewport, string name, Texture2D bodyTexture, RichTextLabel logNode, Texture2D rotorTexture, Vector2[] rotorPositions)
	{
		base.Setup(viewport, name, bodyTexture, logNode);

		// Create rotors
		foreach (var rotorPos in rotorPositions)
		{
			var rotor = new Sprite2D()
			{
				Texture = rotorTexture, 
				Position = rotorPos
			};

			_rotors.Add(rotor);
			AddChild(rotor);
		}

		AddToGroup("drones");
	}

	#endregion Functions

}
