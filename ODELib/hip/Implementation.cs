using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	public enum HBlockType
	{
		Refined,
		NotRefined,
		Both
	}

	/// <summary>
	/// A HiP-HOPS implementation has no direct equivalent in the ODE, but unless optimisation is taking place,
	/// it's essentially just a convenient container.
	/// </summary>
	public class Implementation
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

		public Implementation()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Name { get; set; }
		public bool ExcludeFromOptimisation { get; set; }
		public string Cost { get; set; }
		public string Weight { get; set; }
		public HBlockType HBlockType { get; set; }
		public FailureData FailureData { get; set; }
		public System System { get; set; }

		[XmlIgnore]
		public Component ParentComponent { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Initialises the parent objects for the implementation and all children.
		/// </summary>
		/// <param name="parent">The parent.</param>
		public void Initialise(Component parent)
		{
			ParentComponent = parent;

			if (FailureData != null)
			{
				FailureData.Initialise(parent);
			}

			if (System != null)
			{
				System.Initialise(parent);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Merges the component basic events with those in the FMEA results.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="fmea">The fmea.</param>
		public void MergeWithResults(Component parent, FMEA fmea)
		{
			if (System != null)
			{
				System.MergeWithResults(parent, fmea);
			}

			if (FailureData != null)
			{
				FailureData.MergeWithResults(parent, fmea);
			}
		}

		#endregion Functions

	}
}
