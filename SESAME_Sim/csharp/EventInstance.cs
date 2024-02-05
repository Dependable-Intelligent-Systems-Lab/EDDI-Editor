using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

/// <summary>
/// Instance/occurrence of a particular event
/// </summary>
/// <seealso cref="Godot.GodotObject" />
public partial class EventInstance : GodotObject
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

	public EventInstance()
	{

	}

	//----------------------------------------------------------------------------------------------------//

	public EventInstance(Event template)
	{
		Setup(template);
	}

	#endregion Constructors

	/*****************************************************************************************************/
	/* Properties
	/*****************************************************************************************************/
	#region Properties

	public string Condition { get; private set; }

	// Effectively, this EventInstance is an instance of this Event
	public Event EventTemplate { get; private set; }

	// Set to true to prevent it propagating to other listeners
	public bool IsHandled { get; set; } = false;

	// What raised the event
	public string Originator { get; set; }

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions

	public void Setup(Action messageTriggerEvent) 
	{
		Originator = "ACTION";
		Condition  = messageTriggerEvent.Content;
	}

	//----------------------------------------------------------------------------------------------------//

	public void Setup(Event template)
	{
		EventTemplate = template;
		Condition     = EventTemplate.Condition;
	}

	#endregion Functions

}
