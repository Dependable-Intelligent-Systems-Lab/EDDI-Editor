using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using ODELib;
using ODELib.ode;
using ODELib.hip;
using static Godot.OpenXRInterface;

namespace SESAME_Sim;

public partial class FaultTree : Node2D
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

	public FaultTree()
	{
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public string FaultTreeName { get; set; }
	
	public string FullName => Robot != null ? FaultTreeName + " @ " + Robot.RobotName : FaultTreeName;

	public List<FTNode> Nodes { get; private set; } = new List<FTNode>();

	public Robot Robot { get; set; } // The robot the fault tree belongs to (if applicable; if null, assumed to be system-of-system level)
	
	public FTNode TopNode { get; set; }

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	/// <summary>
	/// Adds a node.
	/// </summary>
	/// <param name="node">The node to add.</param>
	public void AddNode(FTNode node) 
	{ 
		if (!Nodes.Contains(node)) 
		{ 
			Nodes.Add(node);
		}
	}

	//----------------------------------------------------------------------------------------------------//

	/// <summary>
	/// Generates the layout (and connecting lines) for the tree.
	/// </summary>
	public void GenerateLayout()
	{
		TopNode?.GenerateLayout();
		TopNode?.GenerateLines();
	}

	#endregion Functions

	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static FaultTree BuildExampleFaultTree(Robot r=null, RichTextLabel logNode=null)
	{
		FaultTree faultTree = new()
		{
			TopNode = new(),
			Name = "Example Fault Tree",
			FaultTreeName = "Example Fault Tree",
		};
		faultTree.TopNode.Setup(faultTree, "TOP", "nop", null, logNode);
		
		var or  = new FTNode();  or.Setup(faultTree, "OR", "or", faultTree.TopNode, logNode);
		var e1  = new FTNode();  e1.Setup(faultTree, "Motor Failure", "event", or, logNode);
		var and = new FTNode(); and.Setup(faultTree, "AND","and", or, logNode);
		var e2  = new FTNode();  e2.Setup(faultTree, "Comms Failure", "event", and, logNode);
		var e3  = new FTNode();  e3.Setup(faultTree, "Nav Failure", "event", and, logNode);

		faultTree.GenerateLayout();

		r?.SetFaultTree(faultTree);

		// Events/Actions
		string robotName = r != null ? r.RobotName + ": " : "";
		e1.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "Motor Failure occurred"));
		e2.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "Comms Failure occurred"));
		e3.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "Nav Failure occurred"));
		and.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "AND occurred"));
		or.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "OR occurred"));
		faultTree.TopNode.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "TOP occurred"));

		return faultTree;
	}

	//----------------------------------------------------------------------------------------------------//

	public static FaultTree BuildExampleDynamicFaultTree(Robot r = null, RichTextLabel logNode = null)
	{
		FaultTree faultTree = new()
		{
			TopNode = new(),
			Name = "Example Fault Tree",
			FaultTreeName = "Example Fault Tree",
		};
		faultTree.TopNode.Setup(faultTree, "TOP", "nop", null, logNode);

		var or = new FTNode(); or.Setup(faultTree, "OR", "or", faultTree.TopNode, logNode);
		var e1 = new FTNode(); e1.Setup(faultTree, "Motor Failure", "event", or, logNode);
		var pand = new FTNode(); pand.Setup(faultTree, "PAND", "pand", or, logNode);
		var e2 = new FTNode(); e2.Setup(faultTree, "Comms Failure", "event", pand, logNode);
		var e3 = new FTNode(); e3.Setup(faultTree, "Nav Failure", "event", pand, logNode);

		faultTree.GenerateLayout();

		r?.SetFaultTree(faultTree);

		// Events/Actions
		string robotName = r != null ? r.RobotName + ": " : "";
		e1.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "Motor Failure occurred"));
		e2.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "Comms Failure occurred"));
		e3.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "Nav Failure occurred"));
		pand.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "PAND occurred"));
		or.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "OR occurred"));
		faultTree.TopNode.TriggeredActions.Add(new Action(Action.ActionTypeEnum.WARNING, "event", robotName + "TOP occurred"));

		return faultTree;
	}

	//----------------------------------------------------------------------------------------------------//

	public static FaultTree BuildFromODE(ODELib.ode.FaultTree odeFaultTree, Robot r = null, RichTextLabel logNode = null)
	{
		FaultTree faultTree = new FaultTree()
		{
			Name = odeFaultTree.Name,
			FaultTreeName = odeFaultTree.Name,
		};

		faultTree.TopNode = FTNode.BuildFromODE(faultTree, odeFaultTree.TopEvent, null, r, logNode);

		faultTree.GenerateLayout();

		r?.SetFaultTree(faultTree);

		return faultTree;
	}

	#endregion Static Functions
}
