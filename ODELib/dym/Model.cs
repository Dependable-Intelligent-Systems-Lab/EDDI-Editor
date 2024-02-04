using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ODELib.hip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib.dym
{
	/// <summary>
	/// Top-level Dymodia model class. Only State Machines are currently supported.
	/// </summary>
	/// <seealso cref="ODELib.ConverterModel" />
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
			: base(ModelType.DYMODIA)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		public string Name { get; set; }

		public StateMachine StateMachine { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public static Model Load(string filename)
		{
			Model model = new Model();
			using (var zip = ZipFile.Open(filename, ZipArchiveMode.Read))
			{
				// Should be a project file entry; we could interrogate it for contents, but for now we just use it to 
				// check it's a valid Dymodia project.
				var projectEntry = zip.GetEntry("project.json");
				if (projectEntry == null)
				{
					throw new IOException($"Could not load project from zip file '{filename}'");
				}

				// Load the project
				//var project = new Project(projectEntry.Open());

				// Now load the various models etc
				foreach (var file in zip.Entries)
				{
					if (file.FullName.ToLower().EndsWith("usm"))
					{
						// Load state machine -- note that we only load ONE state machine
						using (var stream = file.Open())
						using (var streamReader = new StreamReader(stream))
						{
							string data = streamReader.ReadToEnd();
							var jsObj = JObject.Parse(data);
							model.StateMachine = StateMachine.Deserialise((JObject)jsObj["data"]);
							model.Name = model.StateMachine.Name;
						}
					}
					else if (file.FullName.ToLower().EndsWith("usa"))
					{
						// Load system architecture model
						//using (var stream = file.Open())
						//using (var streamReader = new StreamReader(stream))
						//{
						//	string data = streamReader.ReadToEnd();
						//	var sm = serialiser.Deserialise<SystemModel>(data);
						//	//project.AddSystemModel(sm);
						//}
					}
					else if (file.FullName.ToLower().EndsWith("uft"))
					{
						// Load fault tree
						//using (var stream = file.Open();)
						//using (var streamReader = new StreamReader(stream);)
						//{
						//	string data = streamReader.ReadToEnd();
						//	var ft = serialiser.Deserialise<FaultTree>(data);
						//	var ftm = new FaultTreeModel(ft);
						//	//project.AddFaultTree(ftm);
						//}
					}
				}
			}

			return model;
		}

		#endregion Functions

	}
}
