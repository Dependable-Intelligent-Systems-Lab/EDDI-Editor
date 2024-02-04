using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	public class Component
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

		public Component()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Type { get; set; }
		public string Name { get; set; }
		public string RiskTime { get; set; }
		[XmlIgnore]
		public double RiskTimeValue
		{
			get
			{
				if (double.TryParse(RiskTime, out double val))
				{
					return val;
				}
				return 0;
			}
		}
		public bool IncludeInOptimisation { get; set; }
		[XmlArray]
		[XmlArrayItem("Port")]
		public List<Port> Ports { get; private set; } = new List<Port>();
		[XmlArray]
		[XmlArrayItem(ElementName ="Current")]
		public List<Implementation> Implementations { get; private set; } = new List<Implementation>();

		[XmlIgnore]
		public FMEAComponent FMEAComponent { get; private set; }

		[XmlIgnore]
		public Component ParentComponent { get; private set; }

		[XmlIgnore]
		public Perspective ParentPerspective { get; private set; }

		[XmlIgnore]
		public string FullName
		{
			get
			{
				// Fully qualified name
				if (ParentComponent != null)
				{
					return ParentComponent.FullName + "." + this.Name;
				}
				else if (ParentPerspective != null)
				{
					return ParentPerspective.Name + "::" + this.Name;
				}
				return Name;
			}
			set
			{
				Name = value.Split(new char[] { '.' }).Last();
			}
		}

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Sets the parent object recursively.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public void Initialise(Perspective parent)
		{
			SetParent(parent); // Will set ports too

			if (Implementations.Count > 0)
			{
				Implementations.First().Initialise(this);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Sets the parent object recursively.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public void Initialise(Component parent)
		{
			SetParent(parent); // Will set ports too

			if (Implementations.Count > 0)
			{
				Implementations.First().Initialise(this);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Merges the FMEA basic events with those in the Components.
		/// When loading HiP-HOPS models, we have basic events in the model hierarchy (components) and in the
		/// results (FTA/FMEAs), which means we potentially end up with two copies of each basic event. This 
		/// function is designed to resolve that by matching FMEA basic events to those in the components.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="fmea">The fmea.</param>
		public void MergeWithResults(Perspective parent, FMEA fmea)
		{
			ParentPerspective = parent;

			foreach (var component in fmea.Components)
			{
				if (component.Name == this.FullName)
				{
					FMEAComponent = component;
				}
			}

			// And basic events, and subcomponents...
			if (this.Implementations.Count > 0)
			{
				this.Implementations.First().MergeWithResults(this, fmea);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Merges the FMEA basic events with those in the Components.
		/// When loading HiP-HOPS models, we have basic events in the model hierarchy (components) and in the
		/// results (FTA/FMEAs), which means we potentially end up with two copies of each basic event. This 
		/// function is designed to resolve that by matching FMEA basic events to those in the components.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="fmea">The fmea.</param>
		public void MergeWithResults(Component parent, FMEA fmea)
		{
			SetParent(parent);

			foreach (var component in fmea.Components)
			{
				if (component.Name == this.FullName)
				{
					FMEAComponent = component;
				}
			}

			// And basic events, and subcomponents...
			if (this.Implementations.Count > 0)
			{
				this.Implementations.First().MergeWithResults(this, fmea);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Sets the parent but does not recurse.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public void SetParent(Component parent)
		{
			ParentComponent = parent;

			foreach (var port in Ports)
			{
				port.SetParent(this);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Sets the parent but does not recurse.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public void SetParent(Perspective parent)
		{
			ParentPerspective = parent;

			foreach (var port in Ports)
			{
				port.SetParent(this);
			}
		}

		#endregion Functions

	}
}
