using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	/// <summary>
	/// Top class for the HiP-HOPS system architecture file.
	/// </summary>
	/// <seealso cref="ODELib.ConverterModel" />
	[XmlRoot("Model")]
	public class Model : ConverterModel
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

		public Model()
			: base(ModelType.HIPHOPS)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Name { get; set; }
		public string Version { get; set; }
		public string Description { get; set; }
		public ToolVersion ToolVersion { get; set; }
		public double RiskTime { get; set; }
		public int MaxCutSetOrder { get; set; }
		[XmlArray]
		[XmlArrayItem(ElementName ="Hazard")]
		public List<Hazard> Hazards { get; private set; } = new List<Hazard>();
		[XmlArray]
		[XmlArrayItem("Perspective")]
		public List<Perspective> Perspectives { get; private set; } = new List<Perspective>();

		[XmlIgnore]
		public HipResults Results { get; private set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Merges analyses results data with the original model.
		/// When loading a HH model, we usually load the model architecture first and then the results (FMEA, FTAs).
		/// Since basic events occur in both component failure data and in the FMEA/FTA, we end up with two copies of
		/// what are ostensibly the same failures. This function merges them by looking up the 'correct' basic event
		/// in the Component for each BE in the FMEA. The same must be done for hazards too.
		/// </summary>
		/// <param name="results">The results.</param>
		public void MergeWithResults(HipResults results)
		{
			Results = results;

			// Link components and basic events
			foreach (var perspective in Perspectives)
			{
				foreach (var component in perspective.System.Components)
				{
					component.MergeWithResults(perspective, results.FMEA);
				}
			}

			// Also hazards (and fault trees)
			foreach (var component in results.FMEA.Components)
			{
				foreach (var e in component.Events)
				{
					foreach (var effect in e.Effects)
					{
						foreach (var ft in results.FaultTrees)
						{
							if (ft.Name == effect.Name)
							{
								effect.FaultTree = ft;
							}
						}
						foreach (var hazard in Hazards)
						{
							if (hazard.Name == effect.Name)
							{
								effect.Hazard = hazard;
							}
						}
					}
				}
			}
			foreach (var ft in results.FaultTrees)
			{
				foreach (var hazard in Hazards)
				{
					if (ft.Name == hazard.Name)
					{
						ft.Hazard = hazard;
					}
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Finishes setup after deserialisation etc by setting parent objects.
		/// </summary>
		public void Initialise()
		{
			foreach (var perspective in Perspectives)
			{
				perspective.Initialise(this);
			}
		}

		#endregion Functions

	}

	public struct ToolVersion
	{
		public string Name;
		public string Number;
	}
}
