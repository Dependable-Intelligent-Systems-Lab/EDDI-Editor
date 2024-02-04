using System; 
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ODELib;
using ODELib.hip;
using ODELib.ode;

namespace ODEConverterConsole
{
	class Program
	{
		static void Main(string[] args)
		{
			// Test loading DDI
			var loader = new DDI_Importer();
			var model = loader.ImportFromDDI("SESAME_DDI_example/SESAME_DDI_example.ddi");
		}


		static void TestHiphopsConversion()
		{ 
			ODELib.hip.Model hipmodel = null;

			// Test loading a simple model
			using (var fileStream = File.Open("primarystandbyexample.xml", FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(ODELib.hip.Model));
				hipmodel = (ODELib.hip.Model)serializer.Deserialize(fileStream);
				hipmodel.Initialise();
				var be = hipmodel.Perspectives[0].System.Components[0].Implementations[0].FailureData.BasicEvents[0];
				Console.WriteLine("Model loaded");

				// Try saving it again
				using (var outStream = File.Open("testoutput.xml", FileMode.Create))
				{
					serializer.Serialize(outStream, hipmodel);
				}

			}

			// Test loading a results file
			using (var fileStream = File.Open("primarystandbyexampleResults.xml", FileMode.Open))
			//using (var fileStream = File.Open("nytt60_results.xml", FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(ODELib.hip.HipResults));
				var results = (ODELib.hip.HipResults)serializer.Deserialize(fileStream);
				results.ResolveReferences();
				Console.WriteLine("Results loaded");

				hipmodel.MergeWithResults(results);
				Console.WriteLine("And merged");
			}


			// Now try converting it
			var converter = new ConverterHIPtoODE();
			var ode_model = converter.ConvertHIPtoODE(hipmodel);

			// And save to XML
			var ode_serializer = new XmlSerializer(typeof(ODELib.ode.Model));
			using (var outStream = File.Open("primarystandby_ode_xml.xml", FileMode.Create))
			{
				ode_serializer.Serialize(outStream, ode_model);
			}
		}
	}
}
