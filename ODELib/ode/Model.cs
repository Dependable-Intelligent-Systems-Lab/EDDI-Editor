using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ODELib.ode
{
	[XmlRoot("Model")]
	[DataContract]
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
			: base(ModelType.ODE)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		[DataMember]
		public string Name { get; set; }

		[XmlArray]
		[XmlArrayItem("FailureModels")]
		[DataMember]
		public List<FailureModel> FailureModels { get; private set; } = new List<FailureModel>();

		[XmlArray]
		[XmlArrayItem("System")]
		[DataMember]
		public List<System> SystemElements { get; private set; } = new List<System>();

		[XmlArray]
		[XmlArrayItem("Hazard")]
		[DataMember]
		public List<Hazard> Hazards { get; private set; } = new List<Hazard>();

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Finds a failure with the given name.
		/// </summary>
		/// <param name="fullName">The full name.</param>
		/// <returns>The failure, or null if not found.</returns>
		public Failure LookupFailure(string fullName)
		{
			foreach (var system in SystemElements)
			{
				if (system.LookupFailure(fullName, out var failure))
				{
					return failure;
				}
			}
			return null;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Validates this model. Outputs details to validation.txt.
		/// </summary>
		/// <returns></returns>
		public bool Validate()
		{
			bool result = true;

			using (var logfile = File.Open("validation.txt", FileMode.Create))
			{
				// TO DO

				logfile.Close();
			}
			return result;
		}

		#endregion Functions


		/*****************************************************************************************************/
		/* Static Functions
		/*****************************************************************************************************/
		#region Static Functions

		/// <summary>
		/// Loads (deserialises) a model from an XML file.
		/// </summary>
		/// <param name="xmlFileName">Name of the XML file.</param>
		/// <returns></returns>
		public static ODELib.ode.Model LoadFromXML(string xmlFileName)
		{
			using (var fileStream = File.Open(xmlFileName, FileMode.Open))
			using (var stringStream = new StreamReader(fileStream))
			{
				ODELib.ode.Model model = null;
				string xml = stringStream.ReadToEnd();
				var serializer = new XmlSerializer(typeof(ODELib.ode.Model));

				using (var memoryStream = new MemoryStream((new UTF8Encoding()).GetBytes(xml)))
				{
					model = (ODELib.ode.Model)serializer.Deserialize(memoryStream);
				}
				return model;
			}
		}

		#endregion Static Functions

	}
}
