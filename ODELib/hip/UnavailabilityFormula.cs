using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
	/// <summary>
	/// The unavailability formula stores the type of formula used and the parameter data for it.
	/// Unfortunately, we have to match the way it's stored in the XML files to facilitate serialisation,
	/// hence the rather cumbersome structure here.
	/// </summary>
	public class UnavailabilityFormula
	{
		/*****************************************************************************************************/
		/* Enums/Constants
		/*****************************************************************************************************/
		#region Constants

		public enum UnavailabilityFormulaType
		{
			Constant,
			MTTF_Repair,
			FailureRate_MTTR,
			MTTF_MTTR,
			Fixed,
			Poisson,
			Binomial,
			Dormant,
			Variable
		}

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

		public UnavailabilityFormula()
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlIgnore]
		public UnavailabilityFormulaType Type { get; set; }


		[XmlElement(typeof(UnavailabilityFormula_Constant), ElementName = "Constant")] // There's got to be a better way to do this, but if it ain't broke...
		public UnavailabilityFormula_Constant UnavailabilityFormula_Constant { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_MTTF), ElementName = "MTTF")]
		public UnavailabilityFormula_MTTF UnavailabilityFormula_MTTF { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_MTTR), ElementName = "MTTR")]
		public UnavailabilityFormula_MTTR UnavailabilityFormula_MTTR { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_MTTF_MTTR), ElementName = "MTTFMTTR")]
		public UnavailabilityFormula_MTTF_MTTR UnavailabilityFormula_MTTF_MTTR { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_Fixed), ElementName = "Fixed")]
		public UnavailabilityFormula_Fixed UnavailabilityFormula_Fixed { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_Dormant), ElementName = "Dormant")]
		public UnavailabilityFormula_Dormant UnavailabilityFormula_Dormant { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_Poisson), ElementName = "Poisson")]
		public UnavailabilityFormula_Poisson UnavailabilityFormula_Poisson { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_Binomial), ElementName = "Binomial")]
		public UnavailabilityFormula_Binomial UnavailabilityFormula_Binomial { get; set; } = null;

		[XmlElement(typeof(UnavailabilityFormula_Variable), ElementName = "Variable")]
		public UnavailabilityFormula_Variable UnavailabilityFormula_Variable { get; set; } = null;

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public void Initialise(BasicEvent parent)
		{
			if (UnavailabilityFormula_Constant != null)
			{
				Type = UnavailabilityFormulaType.Constant;
			}
			if (UnavailabilityFormula_MTTF != null)
			{
				Type = UnavailabilityFormulaType.MTTF_Repair;
			}
			if (UnavailabilityFormula_MTTR != null)
			{
				Type = UnavailabilityFormulaType.FailureRate_MTTR;
			}
			if (UnavailabilityFormula_MTTF_MTTR != null)
			{
				Type = UnavailabilityFormulaType.MTTF_MTTR;
			}
			if (UnavailabilityFormula_Fixed != null)
			{
				Type = UnavailabilityFormulaType.Fixed;
			}
			if (UnavailabilityFormula_Dormant != null)
			{
				Type = UnavailabilityFormulaType.Dormant;
			}
			if (UnavailabilityFormula_Poisson != null)
			{
				Type = UnavailabilityFormulaType.Poisson;
			}
			if (UnavailabilityFormula_Binomial != null)
			{
				Type = UnavailabilityFormulaType.Binomial;
			}
			if (UnavailabilityFormula_Variable != null)
			{
				Type = UnavailabilityFormulaType.Variable;
			}
		}

		#endregion Functions
	}

	public class UnavailabilityFormula_Constant
	{
		public double FailureRate { get; set; }
		public double RepairRate { get; set; }
	}

	public class UnavailabilityFormula_MTTF
	{
		public double MTTF { get; set; }
		public double RepairRate { get; set; }
	}

	public class UnavailabilityFormula_MTTR
	{
		public double FailureRate { get; set; }
		public double MTTR { get; set; }
	}

	public class UnavailabilityFormula_MTTF_MTTR
	{
		public double MTTF { get; set; }
		public double MTTR { get; set; }
	}

	public class UnavailabilityFormula_Dormant
	{
		public double FailureRate { get; set; }
		public double MTTR { get; set; }
		public double T { get; set; } // Time between inspections
	}

	public class UnavailabilityFormula_Fixed
	{
		public double Unavailability { get; set; }
	}

	public class UnavailabilityFormula_Poisson
	{
		public double FailureRate { get; set; }
		public double N { get; set; } // Num components active at any time
		public double S { get; set; } // Spare components
		public double T { get; set; } // Time of subsystem operation
	}

	public class UnavailabilityFormula_Binomial
	{
		public double FailureRate { get; set; }
		public double RepairRate { get; set; }
		public double N { get; set; } // Num components active at any time
		public double M { get; set; } // Min components needed to cause subsystem failure
		public double T { get; set; } // Time of operation of the subsystem
	}

	public class UnavailabilityFormula_Variable
	{
		public double Slope1 { get; set; }
		public double Scale1 { get; set; }
		public double Interval1 { get; set; }
		public double Scale2 { get; set; }
		public double Interval2 { get; set; }
		public double Slope3 { get; set; }
		public double Scale3 { get; set; }
	}

}
