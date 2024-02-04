using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	/// <summary>
	/// Simple cross-reference for basic events in cut sets
	/// </summary>
	public class BasicEventRef : FaultTreeNode
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

		public BasicEventRef()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		//[XmlAttribute]
		//public string ID { get; set; } // In FTN

		[XmlIgnore]
		public BasicEventResult BasicEvent { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Resolves the reference and sets the 'real' basic event.
		/// When loading an FTA, the basic events are really just shallow references to the 'real' basic events
		/// in the FMEA (or technically the Components; see BasicEvent.MergeWithResults()). This function can
		/// be called once everything is loaded to resolve the references and make sure everything links up.
		/// </summary>
		/// <param name="fmea">The fmea.</param>
		public override void ResolveReferences(FMEA fmea)
		{
			foreach (var component in fmea.Components)
			{
				foreach (var basicEvent in component.Events)
				{
					if (basicEvent.ID == this.ID)
					{
						BasicEvent = basicEvent;
						return;
					}
				}
			}
		}

		#endregion Functions

	}
}
