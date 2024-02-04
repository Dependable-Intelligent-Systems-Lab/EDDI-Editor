using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
	public class SystemVM
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

		public SystemVM(ODELib.ode.System system)
		{
			OdeSystem = system;

			if (system.AssuranceLevel != null)
			{
				AssuranceLevel = new AssuranceLevelVM(system.AssuranceLevel);
			}

			foreach (var signal in OdeSystem.Signals)
			{
				Signals.Add(new SignalVM(signal));
			}
			foreach (var port in OdeSystem.Ports)
			{
				Ports.Add(new PortVM(port));
			}
			foreach (var subsystem in OdeSystem.Subsystems)
			{
				Subsystems.Add(new SystemVM(subsystem));
			}
			foreach (var fmodel in OdeSystem.FailureModels)
			{
				if (fmodel is ODELib.ode.StateMachine sm)
				{
					FailureModels.Add(new StateMachineVM(sm));
				}
				else if (fmodel is ODELib.ode.FaultTree ft)
				{
					FailureModels.Add(new FaultTreeVM(ft));
				}
				else
				{
					FailureModels.Add(new FailureModelVM(fmodel));
				}
			}
			foreach (var monitor in OdeSystem.EventMonitors)
			{
				EventMonitors.Add(new EventMonitorVM(monitor));
			}

			Items.Add(Signals);
			Items.Add(Ports);
			Items.Add(Subsystems);
			Items.Add(FailureModels);
			Items.Add(EventMonitors);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public ODELib.ode.System OdeSystem { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		public ObservableList Items { get; set; } = new ObservableList();

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => OdeSystem.Name; set => OdeSystem.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Key-Value Map")]
		[Description("Key-Value Map")]
		public SerialisableDictionary<string, SerialisableDictionary<string, string>> KVM => OdeSystem.KeyValueMap;

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Signals")]
		[Description("Signals")]
		[ExpandableObject]
		public ExpandableList Signals { get; private set; } = new ExpandableList("Signals");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Ports")]
		[Description("Ports")]
		[ExpandableObject]
		public ExpandableList Ports { get; private set; } = new ExpandableList("Ports");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Subsystems")]
		[Description("Subsystems")]
		[ExpandableObject]
		public ExpandableList Subsystems { get; private set; } = new ExpandableList("Subsystems");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Assurance Level")]
		[Description("Assurance Level")]
		public AssuranceLevelVM AssuranceLevel { get; set; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Failure Models")]
		[Description("Failure Models")]
		[ExpandableObject]
		public ExpandableList FailureModels { get; private set; } = new ExpandableList("Failure Models");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Event Monitors")]
		[Description("Event Monitors")]
		[ExpandableObject]
		public ExpandableList EventMonitors { get; private set; } = new ExpandableList("Event Monitors");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public void AddFailureModels(ODELib.ode.System sys)
		{
			if (sys != null)
			{
				foreach (var fm in sys.FailureModels)
				{
					OdeSystem.FailureModels.Add(fm);

					// Create VM for it
					if (fm is ODELib.ode.StateMachine sm)
					{
						FailureModels.Add(new StateMachineVM(sm));
					}
					else
					{
						FailureModels.Add(new FailureModelVM(fm));
					}
				}
			}

		}

		//----------------------------------------------------------------------------------------------------//

		public void AddSubsystem(ODELib.ode.System sys)
		{
			if (sys != null)
			{
				OdeSystem.Subsystems.Add(sys);

				// Create VM for it
				var vm = new SystemVM(sys);
				Subsystems.Add(vm);
			}

		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Replaces a (dummy) system with an imported one.
		/// The idea is that you can have a placeholder system that gets replaced by an entire imported system hierarchy/model.
		/// So for example you could have "fuel subsystem" that gets replaced by a hierarchy involving tanks, pumps, sensors etc.
		/// </summary>
		/// <param name="existing">The existing.</param>
		/// <param name="replacement">The replacement.</param>
		/// <returns></returns>
		public string ReplaceSystem(SystemVM existing, ODELib.ode.System replacement)
		{
			if (!Subsystems.Contains(existing))
			{
				return $"Could not find system '{existing.Name}' to replace.";
			}

			// Check that the ports match (NB replacement can have *more* ports, just not less)
			foreach (PortVM port in existing.Ports)
			{
				bool found = false;
				foreach (var rport in replacement.Ports)
				{
					if (rport.Name == port.Name)
					{
						found = true;
					}
				}
				if (!found)
				{
					return $"Port mismatch -- no corresponding port for '{port.Name}'";
				}
			}

			Subsystems.Remove(existing);
			OdeSystem.Subsystems.Remove(existing.OdeSystem);
			Subsystems.Add(new SystemVM(replacement));
			OdeSystem.Subsystems.Add(replacement);
			return "OK";
		}

		//----------------------------------------------------------------------------------------------------//

		#endregion Functions

	}
}
