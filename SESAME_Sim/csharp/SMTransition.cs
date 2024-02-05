using Godot;
using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static System.TimeZoneInfo;

namespace SESAME_Sim
{
	public partial class SMTransition : Node2D, IEventListener
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

		protected Line2D _arrow; // The arrow 
		protected Label _label; // The name of the transition
		protected RichTextLabel _logNode;

		#endregion Data

		/*****************************************************************************************************/
		/* Constructors
		/*****************************************************************************************************/
		#region Constructors

		public SMTransition()
		{
			EventProcessor.GetSingleton().RegisterListener(this);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public StateMachine ParentStateMachine { get; private set; }

		public SMState FromState { get; private set; }

		public SMState ToState { get; private set; }

		public string TransitionName { get; set; }

		public List<Event> TriggeringEvents { get; private set; } = new List<Event>(); // Events that can trigger this transition

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Activates/executes this transition (and performs a state change)
		/// </summary>
		public void Activate()
		{
			if (FromState.IsActive)
			{
				if (_logNode != null)
				{
					_logNode.Text += $"[color=magenta]Transition '{TransitionName}' has occurred\n[/color]";
				}

				FromState.Deactivate();
				ToState.Activate();
			}
		}

		//----------------------------------------------------------------------------------------------------//


		/// <summary>
		/// Check collision between a line (l1->l2) and a rectangle (i.e., list of corner points)
		/// </summary>
		/// <returns>The closest point of collision to l1, or null if no collision</returns>
		public Vector2? CollisionLineVsRectangle(Vector2 l1, Vector2 l2, Vector2[] corners)
		{
			var collisions = new List<Vector2>();
			var col = IntersectionTrianglesMethod(l1, l2, corners[0], corners[1]);
			if (col != null)
			{
				collisions.Add((Vector2)col);
			}
			col = IntersectionTrianglesMethod(l1, l2, corners[1], corners[2]);
			if (col != null)
			{
				collisions.Add((Vector2)col);
			}
			col = IntersectionTrianglesMethod(l1, l2, corners[2], corners[3]);
			if (col != null)
			{
				collisions.Add((Vector2)col);
			}
			col = IntersectionTrianglesMethod(l1, l2, corners[3], corners[0]);
			if (col != null)
			{
				collisions.Add((Vector2)col);
			}

			// We might have 2 collisions
			if (collisions.Count == 0)
			{
				return null;
			}
			else if (collisions.Count == 1)
			{
				return collisions[0];
			}
			else
			{
				// Find the closest to the start point
				var d1 = (collisions[0] - l1).LengthSquared();
				var d2 = (collisions[1] - l1).LengthSquared();
				if (d1 < d2)
				{ 
					return collisions[0]; 
				}
				else 
				{ 
					return collisions[1]; 
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Line/Line intersection using the triangle area method from The Orange Book (Real-Time Collision Detection).
		/// This has a minor bug when the line intersects at the corner so the Triangles method (below) is preferred.
		/// </summary>
		/// <param name="startPoint">Start of intersecting line</param>
		/// <param name="endPoint">End of intersecting line</param>
		/// <param name="prev">Start of line being tested against</param>
		/// <param name="next">End of line being tested against</param>
		/// <returns></returns>
		public Vector2? IntersectionOrangeBookMethod(Vector2 startPoint, Vector2 endPoint, Vector2 prev, Vector2 next)
		{
			// From The Orange Book p152-3

			float a1 = (startPoint.X - next.X) * (endPoint.Y - next.Y) - (startPoint.Y - next.Y) * (endPoint.X - next.X); 
			float a2 = (startPoint.X - prev.X) * (endPoint.Y - prev.Y) - (startPoint.Y - prev.Y) * (endPoint.X - prev.X); 
			float test = a1 * a2;

			if (a1 * a2 < 0.0f)
			{
				float a3 = (prev.X - startPoint.X) * (next.Y - startPoint.Y) - (prev.Y - startPoint.Y) * (next.X - startPoint.X); 
				float a4 = a3 + a2 - a1;
				if (a3 * a4 < 0.0f)
				{
					return (endPoint - startPoint) * (a3 / (a3 - a4)) + startPoint;
				}
			}

			return null;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Line/Line intersection using the Triangles algorithm
		/// </summary>
		/// <param name="startPoint">Start of intersecting line</param>
		/// <param name="endPoint">End of intersecting line</param>
		/// <param name="prev">Start of line being tested against</param>
		/// <param name="next">End of line being tested against</param>
		/// <returns></returns>
		public Vector2? IntersectionTrianglesMethod(Vector2 startPoint, Vector2 endPoint, Vector2 prev, Vector2 next)
		{
			float d = (((next.Y - prev.Y) * (endPoint.X - startPoint.X)) -
					   ((next.X - prev.X) * (endPoint.Y - startPoint.Y)));

			if (d != 0)
			{
				float u1n = (((next.X - prev.X) * (startPoint.Y - prev.Y)) -
							 ((next.Y - prev.Y) * (startPoint.X - prev.X)));
				float u2n = (((endPoint.X - startPoint.X) * (startPoint.Y - prev.Y)) -
							 ((endPoint.Y - startPoint.Y) * (startPoint.X - prev.X)));
				float u1 = u1n / d;
				float u2 = u2n / d;

				// if u1 and u2 are between 0 and 1 then the line segments intersect
				if ((u1 >= 0) && (u1 <= 1) && (u2 >= 0) && (u2 <= 1))
				{
					return (endPoint - startPoint) * u1 + startPoint;
				}
			}

			return null;
		}

		//----------------------------------------------------------------------------------------------------//

		public void Setup(StateMachine parentStateMachine, string name, SMState from, SMState to, RichTextLabel logNode=null)
		{
			Name               = name;
			TransitionName     = name;
			ParentStateMachine = parentStateMachine;
			parentStateMachine.AddTransition(this);

			_logNode = logNode;

			FromState = from;
			ToState = to;
			from.OutgoingTransitions.Add(this);
			to.IncomingTransitions.Add(this);

			// Set up the basics
			_label = new Label()
			{
				Text = name,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				Position = (from.Position + to.Position)/2 - new Vector2(50,0),
				Size = new Vector2(100, 30)
			};

			_arrow = new Line2D()
			{
				Texture = GD.Load<Texture2D>("res://textures/line.png"),
				TextureMode = Line2D.LineTextureMode.Stretch,
				JointMode = Line2D.LineJointMode.Round,
				EndCapMode = Line2D.LineCapMode.Round,
				Width = 5
			};

			AddChild(_arrow);
			AddChild(_label);

			// Set up the points
			_arrow.ClearPoints();
			var start_ = CollisionLineVsRectangle(from.Position, to.Position, from.GetCorners());
			var end_ = CollisionLineVsRectangle(from.Position, to.Position, to.GetCorners());
			if (start_ != null && end_ != null)
			{
				var start               = (Vector2)start_;
				var end                 = (Vector2)end_;
				var line_vec            = end - start;
				var line_len            = Math.Max(1, line_vec.Length()); // Avoid /0
				var arrow_start_percent = (line_len - 15) / line_len;
				var arrow_start         = start + line_vec * arrow_start_percent;
				var normalised_line_vec = line_vec.Normalized();
				var normal              = new Vector2(-normalised_line_vec.Y, normalised_line_vec.X);

				_arrow.AddPoint(start);
				_arrow.AddPoint(arrow_start);
				_arrow.AddPoint(arrow_start + normal * 15);
				_arrow.AddPoint(end);
				_arrow.AddPoint(arrow_start - normal * 15);
				_arrow.AddPoint(arrow_start);
			}

			ParentStateMachine.AddChild(this);
		}

		#endregion Functions


		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/
		#region Static Functions

		public static SMTransition BuildFromODE(StateMachine stateMachine, ODELib.ode.Transition odeTransition, SMState from, SMState to, Robot robot = null, RichTextLabel logNode = null)
		{
			if (odeTransition is null)
			{
				throw new ArgumentNullException(nameof(odeTransition));
			}

			SMTransition transition = new();
			transition.Setup(stateMachine, odeTransition.Name, from, to, logNode);

			// Events/Actions
			foreach (var e in odeTransition.Triggers)
			{
				// Make a new event
				transition.TriggeringEvents.Add(Event.BuildFromODE(e, robot));
			}

			return transition;
		}

		//----------------------------------------------------------------------------------------------------//


		#endregion Static Functions

		/*****************************************************************************************************/
		/* IEventListener
		/*****************************************************************************************************/
		#region IEventListener

		public string GetID()
		{
			return "SMT:" + Name;
		}

		//----------------------------------------------------------------------------------------------------//

		public void HandleEvent(EventInstance ei)
		{
			foreach (var e in TriggeringEvents)
			{
				if (e.Condition == ei.Condition)
				{
					Activate(); // The condition is fulfilled - activate the transition
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		public void HandleAction(Action a)
		{
			if (a.Target == GetID())
			{
				// Do something with it
			}
		}

		#endregion IEventListener

	}
}
