using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib
{
	/// <summary>
	/// Parent class of the various model-to-model converters
	/// </summary>
	public abstract class Converter
	{
		/*****************************************************************************************************/
		/* Enums/Constants
		/*****************************************************************************************************/
		#region Constants

		public enum ConverterType
		{
			HIPHOPS_TO_ODE,
			DYMODIA_TO_ODE,
			ODE_TO_DYMODIA,
			ODE_TO_HIPHOPS,
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

		public Converter(ConverterType type)
		{
			Type = type;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public ConverterType Type { get; private set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public abstract ConverterModel Convert(ConverterModel inputModel);

		#endregion Functions

	}
}
