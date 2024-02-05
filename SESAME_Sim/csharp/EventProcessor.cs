using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SESAME_Sim;

/// <summary>
/// This is a class that handles event/action processing without traversing the scene graph. Basically
/// the 'brain' of the MRS.
/// </summary>
/// <seealso cref="Godot.GodotObject" />
public partial class EventProcessor : GodotObject
{
	/*****************************************************************************************************/
	/* Enums/Constants
	/*****************************************************************************************************/
	#region Constants

	private static EventProcessor _singleton;

	#endregion Constants

	/*****************************************************************************************************/
	/* Data
	/*****************************************************************************************************/
	#region Data

	private List<IEventListener> _listeners = new();

	#endregion Data

	/*****************************************************************************************************/
	/* Constructors
	/*****************************************************************************************************/
	#region Constructors

	public EventProcessor()
	{
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

	public void ProcessAction(Action a)
	{
		if (a.ContentType == null || a.ContentType == "")
		{
			a.ContentType = "event";
		}

		// Special case for warning actions with 'event' as the content type
		// These become new condition events. The Content should be the event condition.
		if (a != null && a.ActionType == Action.ActionTypeEnum.WARNING && a.ContentType.ToLower() == "event") 
		{
			var ei = new EventInstance();
			ei.Setup(a);
			ProcessEvent(ei);
			return;
		}

		foreach (var listener in _listeners) 
		{ 
			if (listener.GetID() == a.Target)
			{
				listener.HandleAction(a);
			}
		}
	}

	//----------------------------------------------------------------------------------------------------//

	public void ProcessEvent(EventInstance ei)
	{
		foreach (var listener in _listeners) 
		{ 
			listener.HandleEvent(ei);
			if (ei.IsHandled)
			{
				break;
			}
		}
	}

	//----------------------------------------------------------------------------------------------------//

	public void RegisterListener(IEventListener listener)
	{
		_listeners.Add(listener);
	}

	//----------------------------------------------------------------------------------------------------//

	public void UnregisterListener(IEventListener listener)
	{
		_listeners.Remove(listener);
	}

	#endregion Functions

	/*****************************************************************************************************/
	/* Static Functions
	/*****************************************************************************************************/
	#region Static Functions

	public static EventProcessor GetSingleton()
	{
		if (_singleton == null)
		{
			_singleton = new EventProcessor();
		}
		return _singleton;
	}

	#endregion Static Functions
}
