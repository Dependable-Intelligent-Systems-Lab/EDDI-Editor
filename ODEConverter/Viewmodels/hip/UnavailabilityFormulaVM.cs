using ODELib.hip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.hip
{
	[DisplayName("HiP-HOPS Unavailability Formula")]
	public class UnavailabilityFormulaVM
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

		public UnavailabilityFormulaVM(UnavailabilityFormula formula)
		{
			HipFormula = formula;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private UnavailabilityFormula HipFormula { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		// Formula params etc

		public double FailureRate
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Constant:          return HipFormula.UnavailabilityFormula_Constant.FailureRate;
					case UnavailabilityFormula.UnavailabilityFormulaType.FailureRate_MTTR:  return HipFormula.UnavailabilityFormula_MTTR.FailureRate;
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:           return HipFormula.UnavailabilityFormula_Poisson.FailureRate;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:          return HipFormula.UnavailabilityFormula_Binomial.FailureRate;
					case UnavailabilityFormula.UnavailabilityFormulaType.Dormant:           return HipFormula.UnavailabilityFormula_Dormant.FailureRate;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Constant:          HipFormula.UnavailabilityFormula_Constant.FailureRate = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.FailureRate_MTTR:  HipFormula.UnavailabilityFormula_MTTR.FailureRate = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:           HipFormula.UnavailabilityFormula_Poisson.FailureRate = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:          HipFormula.UnavailabilityFormula_Binomial.FailureRate = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Dormant:           HipFormula.UnavailabilityFormula_Dormant.FailureRate = value; break;
				}
			}
		}

		public double RepairRate
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Constant:			return HipFormula.UnavailabilityFormula_Constant.RepairRate;
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_Repair:		return HipFormula.UnavailabilityFormula_MTTF.RepairRate;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:			return HipFormula.UnavailabilityFormula_Binomial.RepairRate;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Constant:			HipFormula.UnavailabilityFormula_Constant.RepairRate = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_Repair:		HipFormula.UnavailabilityFormula_MTTF.RepairRate = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:			HipFormula.UnavailabilityFormula_Binomial.RepairRate = value; break;
				}
			}
		}

		public double MTTF
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_Repair:		return HipFormula.UnavailabilityFormula_MTTF.MTTF;
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_MTTR:			return HipFormula.UnavailabilityFormula_MTTF_MTTR.MTTF;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_Repair:		HipFormula.UnavailabilityFormula_MTTF.MTTF = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_MTTR:			HipFormula.UnavailabilityFormula_MTTF_MTTR.MTTF = value; break;
				}
			}
		}

		public double MTTR
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.FailureRate_MTTR:	return HipFormula.UnavailabilityFormula_MTTR.MTTR;
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_MTTR:			return HipFormula.UnavailabilityFormula_MTTF_MTTR.MTTR;
					case UnavailabilityFormula.UnavailabilityFormulaType.Dormant:			return HipFormula.UnavailabilityFormula_Dormant.MTTR;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.FailureRate_MTTR:	HipFormula.UnavailabilityFormula_MTTR.MTTR = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.MTTF_MTTR:			HipFormula.UnavailabilityFormula_MTTF_MTTR.MTTR = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Dormant:			HipFormula.UnavailabilityFormula_Dormant.MTTR = value; break;
				}
			}
		}

		public double Unavailability
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Fixed:				return HipFormula.UnavailabilityFormula_Fixed.Unavailability;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Fixed:				HipFormula.UnavailabilityFormula_Fixed.Unavailability = value; break;
				}
			}
		}

		public double T
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:			return HipFormula.UnavailabilityFormula_Poisson.T;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:			return HipFormula.UnavailabilityFormula_Binomial.T;
					case UnavailabilityFormula.UnavailabilityFormulaType.Dormant:			return HipFormula.UnavailabilityFormula_Dormant.T;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:			HipFormula.UnavailabilityFormula_Poisson.T = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:			HipFormula.UnavailabilityFormula_Binomial.T = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Dormant:			HipFormula.UnavailabilityFormula_Dormant.T = value; break;
				}
			}
		}

		public double N
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:			return HipFormula.UnavailabilityFormula_Poisson.N;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:			return HipFormula.UnavailabilityFormula_Binomial.N;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:			HipFormula.UnavailabilityFormula_Poisson.N = value; break;
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial:			HipFormula.UnavailabilityFormula_Binomial.N = value; break;
				}
			}
		}

		public double S
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:			return HipFormula.UnavailabilityFormula_Poisson.S;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Poisson:			HipFormula.UnavailabilityFormula_Poisson.S = value; break;
				}
			}
		}

		public double M
		{
			get
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial: return HipFormula.UnavailabilityFormula_Binomial.M;
					default: return 0;
				}
			}
			set
			{
				switch (HipFormula.Type)
				{
					case UnavailabilityFormula.UnavailabilityFormulaType.Binomial: HipFormula.UnavailabilityFormula_Binomial.M = value; break;
				}
			}
		}

		public double Slope1 { get => HipFormula.UnavailabilityFormula_Variable?.Slope1 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Slope1 = value; } }
		public double Slope3 { get => HipFormula.UnavailabilityFormula_Variable?.Slope3 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Slope3 = value; } }
		public double Scale1 { get => HipFormula.UnavailabilityFormula_Variable?.Scale1 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Scale1 = value; } }
		public double Scale2 { get => HipFormula.UnavailabilityFormula_Variable?.Scale2 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Scale2 = value; } }
		public double Scale3 { get => HipFormula.UnavailabilityFormula_Variable?.Scale3 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Scale3 = value; } }
		public double Interval1 { get => HipFormula.UnavailabilityFormula_Variable?.Interval1 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Interval1 = value; } }
		public double Interval2 { get => HipFormula.UnavailabilityFormula_Variable?.Interval2 ?? 0; set { if (HipFormula.UnavailabilityFormula_Variable != null) HipFormula.UnavailabilityFormula_Variable.Interval2 = value; } }

	#endregion Properties

	/*****************************************************************************************************/
	/* Functions
	/*****************************************************************************************************/
	#region Functions
	#endregion Functions

}
}
