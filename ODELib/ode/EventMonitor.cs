using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
	public class EventMonitor : Base
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
		public EventMonitor() { }

		public EventMonitor(string name)
			: base(name)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[XmlAttribute("variableName")]
		public string VariableName { get; set; }

		[XmlAttribute("samplingRate")]
		public double SamplingRate { get; set; }

		[XmlAttribute("dataType")]
		public DataType DataType { get; set; }

		[XmlAttribute("sampleRateUnit")]
		public TimeUnit SampleRateUnit { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}

	public enum DataType
	{
		IntegerData,
		RealData,
		LogicalData
	}

	public enum TimeUnit
	{
		Millisecond = 0,
		Second = 1,
		Minute = 2,
		Hour = 3,
		Day = 4,
		Week = 5,
		Month = 6,
		Year = 7
	}
}
