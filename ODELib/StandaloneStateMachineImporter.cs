using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ODELib.ode;

namespace ODELib
{
	/// <summary>
	/// Simple importer for simple JSON-based state machines.
	/// Format of the JSON file is basically:
	/// {
	///		name : "name of state machine",
	///		states :
	///		[
	///			{
	///				"name" : "name of state",
	///				"position" : [100, 100],	// X/Y coords
	///				"colour" : [r,g,b],			// RGB colour (floats, 0.0 - 1.0)
	///				"start" : true/false,		// Starting state? (should only be one!)
	///				"fail" : true/false,		// Failure state?
	///				"onentry" :					// Actions to perform on entry to the state
	///				[
	///					{
	///						"name" : "name of action",	
	///						"type : "type of action",		// e.g. FunctionAction
	///						"function" : "what to do"		// Action to perform
	///					}
	///				]
	///			},
	///			...
	///		],
	///		transitions :
	///		[
	///			{
	///				"name" : "name of transition",
	///				"from" : "from state name",
	///				"to"   : "to state name",
	///				"trigger" : "events that cause the transition"	// Use semicolons to join multiple
	///			},
	///			...
	///		]
	/// }
	/// </summary>
	public static class StandaloneStateMachineImporter
	{
		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/

		/// <summary>
		/// Imports a state machine from a JSON file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public static ODELib.ode.Model ImportFromJSON(string fileName)
		{
			var json = JObject.Parse(File.ReadAllText(fileName));

			ODELib.ode.StateMachine stateMachine = new ode.StateMachine();

			foreach (var item in json)
			{
				switch (item.Key)
				{
					case "name":
						stateMachine.Name = item.Value.ToString();
						break;
					case "states":
						foreach (JObject jstate in item.Value)
						{
							var state = new ode.State();
							foreach (var subitem in jstate)
							{
								switch (subitem.Key)
								{
									case "name":
										state.Name = subitem.Value.ToString();
										break;
									case "position":
										var x = (float)((JArray)subitem.Value)[0];
										var y = (float)((JArray)subitem.Value)[1];
										state.KeyValueMap["Position"] = new SerialisableDictionary<string, string>();
										state.KeyValueMap["Position"]["x"] = x.ToString();
										state.KeyValueMap["Position"]["y"] = y.ToString();
										break;
									case "colour":
										var r = (float)((JArray)subitem.Value)[0];
										var g = (float)((JArray)subitem.Value)[1];
										var b = (float)((JArray)subitem.Value)[1];
										state.KeyValueMap["Colour"] = new SerialisableDictionary<string, string>();
										state.KeyValueMap["Colour"]["r"] = r.ToString();
										state.KeyValueMap["Colour"]["g"] = g.ToString();
										state.KeyValueMap["Colour"]["b"] = b.ToString();
										break;
									case "start":
										if (subitem.Value.ToString().ToLower() == "true")
										{
											state.IsInitialState = true;
										}
										break;
									case "fail":
										if (subitem.Value.ToString().ToLower() == "true")
										{
											state.IsFailState = true;
										}
										break;
									case "onentry":
										foreach (JObject jaction in subitem.Value)
										{
											var action = new FunctionAction();
											foreach (var subsubitem in jaction)
											{
												switch (subsubitem.Key)
												{
													case "name": action.Name = subsubitem.Value.ToString(); break;
													case "function": action.Function = subsubitem.Value.ToString(); break;
												}
											}
											state.OnEntry.Add(action);
										}
										break;

								}
							}
							stateMachine.States.Add(state);
						}
						break;
					case "transitions":
						foreach (JObject jtransition in item.Value)
						{
							var transition = new ode.Transition();
							foreach (var subitem in jtransition)
							{
								switch (subitem.Key)
								{
									case "name":
										transition.Name = subitem.Value.ToString();
										break;
									case "from":
										var fromState = stateMachine.States.Where(x => x.Name == subitem.Value.ToString()).First();
										transition.FromState = fromState;
										break;
									case "to":
										var toState = stateMachine.States.Where(x => x.Name == subitem.Value.ToString()).First();
										transition.ToState = toState;
										break;
									case "trigger":
										transition.Trigger = subitem.Value.ToString();

										// Make triggering events from these
										if (!string.IsNullOrEmpty(transition.Trigger))
										{
											string[] triggers = transition.Trigger.Split(new char[] { ';' });
											foreach (string trigger in triggers)
											{
												var conditionEvent = new ConditionEvent("Condition=" + trigger.Trim()) { Condition = trigger.Trim() };
												transition.Triggers.Add(conditionEvent);
											}
										}

										break;

								}
							}
							stateMachine.Transitions.Add(transition);
						}
						break;
				}
			}

			var model = new ODELib.ode.Model();
			var system = new ODELib.ode.System("JSON State Machine");
			model.SystemElements.Add(system);
			system.FailureModels.Add(stateMachine);

			return model;
		}
	}
}
