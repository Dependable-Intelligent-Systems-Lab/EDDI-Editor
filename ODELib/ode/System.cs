using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ODELib.ode
{
	[DataContract]
	public class System : Base
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

		public System() { }
		public System(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlAttribute("overallLifetime")]
		public double overallLifetime { get; set; }

		[XmlAttribute("overallLifetimeTimeUnit")]
		public TimeUnit overallLifetimeTimeUnit { get; set; }

		//public List<SystemBoundary> SystemBoundaries { get; private set; } = new List<SystemBoundary>();
		//public List<Context> Contexts { get; private set; } = new List<Context>();
		//public List<Configuration> Configurations { get; private set; } = new List<Configuration>();
		//public List<ServiceConfiguration> ServiceConfigurations { get; private set; } = new List<ServiceConfiguration>();
		//public ServiceConfiguration ActiveServiceConfiguration { get; set; }
		
		[XmlArray(ElementName ="signals")]
		[XmlArrayItem("Signal")]
		[DataMember]
		public List<Signal> Signals { get; private set; } = new List<Signal>();

		[XmlArray(ElementName ="ports")]
		[XmlArrayItem("Port")]
		[DataMember]
		public List<Port> Ports { get; private set; } = new List<Port>();

		[XmlArray(ElementName ="subSystems")]
		[XmlArrayItem("System")]
		[DataMember]
		public List<System> Subsystems { get; private set; } = new List<System>();

		[XmlIgnore]
		public System Parent { get; set; } // Parent system

		//public List<Function> RealisedFunctions { get; private set; } = new List<Function>();
		//public Asset Asset { get; set; }
		//public List<DependabilityRequirement> DependabilityRequirements { get; private set; } = new List<DependabilityRequirement>();

		[XmlElement(ElementName ="assuranceLevel")]
		public AssuranceLevel AssuranceLevel { get; set; }

		//public List<Standard> AppliedStandards { get; private set; } = new List<Standard>();

		[DataMember]
		[XmlArray(ElementName="failureModels")]
		[XmlArrayItem(typeof(FMEA), ElementName = "FMEA")]
		[XmlArrayItem(typeof(FaultTree), ElementName = "FaultTree")]
		[XmlArrayItem(typeof(FailureModel), ElementName = "FailureModel")]
		[XmlArrayItem(typeof(StateMachine), ElementName = "StateMachine")]
		public List<FailureModel> FailureModels { get; private set; } = new List<FailureModel>();

		[XmlArray(ElementName ="eventMonitors")]
		[XmlArrayItem("EventMonitor")]
		[DataMember]
		public List<EventMonitor> EventMonitors { get; private set; } = new List<EventMonitor>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Gets the port with the given name, or null if not found.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		public Port GetPort(string name)
		{
			var ports = Ports.Where(x => x.Name == name);
			return ports.FirstOrDefault();
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Looks up the failure with the given name and returns it via out parameter. Function returns true/false
		/// depending on whether the failure is found or not.
		/// </summary>
		/// <param name="fullName">The full name.</param>
		/// <param name="failure">The failure.</param>
		/// <returns></returns>
		internal bool LookupFailure(string fullName, out Failure failure)
		{
			failure = null;

			foreach (var fmodel in FailureModels)
			{
				var searchResults = fmodel.Failures.Where(x => x.Name == fullName);
				if (searchResults.Count() > 0)
				{
					failure = searchResults.First();
					return true;
				}
			}

			// Try subcomponents
			foreach (var subcomponent in Subsystems)
			{
				if (subcomponent.LookupFailure(fullName, out failure))
				{
					return true;
				}
			}

			return false;
		}

		#endregion Functions


		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/
		#region Static Functions

		#endregion Static Functions

	}
}
