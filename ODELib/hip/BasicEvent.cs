using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	public class BasicEvent
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

		public BasicEvent()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string ID { get; set; }
		public string Name { get; set; }
		public UnavailabilityFormula UnavailabilityFormula { get; set; }


		[XmlIgnore]
		public Component ParentComponent { get; set; }

		[XmlIgnore]
		public BasicEventResult BasicEventResult { get; set; }

		[XmlIgnore]
		public string FullName
		{
			get
			{
				string name = ParentComponent?.FullName ?? "";
				name += "." + this.Name;
				return name;
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
		/// Merges a placeholder basic event with the 'real' basic event from the results.
		/// When loading a HH model, we usually load the model architecture first and then the results (FMEA, FTAs).
		/// Since basic events occur in both component failure data and in the FMEA/FTA, we end up with two copies of
		/// what are ostensibly the same failures. This function merges them by looking up the 'correct' basic event
		/// in the Component for each BE in the FMEA.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="fmea">The fmea.</param>
		public void MergeWithResults(Component parent, FMEA fmea)
		{
			ParentComponent = parent;

			foreach (var component in fmea.Components)
			{
				foreach (var e in component.Events)
				{
					// Add default perspective namespace if not present in name
					string eventName = e.Name;
					if (eventName.IndexOf("::") < 0)
					{
						eventName = "Default::" + eventName;
					}

					if (eventName == this.FullName) 
					{
						BasicEventResult = e;
						e.OriginalBasicEvent = this;
						ID = e.ID;
					}
				}
			}
		}

		#endregion Functions

	}
}
