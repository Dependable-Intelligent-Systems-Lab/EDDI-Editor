using Godot;
using Godot.NativeInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim.csharp
{
	/// <summary>
	/// Connecting line between fault tree nodes
	/// </summary>
	/// <seealso cref="Godot.Line2D" />
	public partial class FTLine : Line2D
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

		public FTLine()
		{
			JointMode   = LineJointMode.Round;
			Texture     = GD.Load<Texture2D>("res://textures/line.png");
			TextureMode = LineTextureMode.Stretch;
			Width       = 3;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties
		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public void Join(FTNode parent, FTNode child)
		{
			Points = new Vector2[4]
			{
				new(parent.X, parent.Y + 100),
				new(parent.X, (child.Y + parent.Y) / 2 + 25),
				new(child.X, (child.Y + parent.Y) / 2 + 25),
				new(child.X, child.Y - 50)
			};
		}

		#endregion Functions

	}
}
