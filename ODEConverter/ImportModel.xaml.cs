using Microsoft.Win32;
using ODELib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ODEConverter
{
	/// <summary>
	/// Interaction logic for ImportModel.xaml
	/// </summary>
	public partial class ImportModel : Window
	{
		private ConverterModel importedModel;
		private Viewmodels.hip.ModelVM importedHIPModelVM;
		private Viewmodels.ode.ModelVM importedODEModelVM;
		private Viewmodels.dym.ModelVM importedDYMModelVM;

		public ConverterModel ImportedModel { get => importedModel; private set => importedModel = value; }

		//----------------------------------------------------------------------------------------------------//

		public ImportModel()
		{
			InitializeComponent();

			this.openResultsFile.IsEnabled = false;
			this.convertModel.IsEnabled = false;
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_SelectModelFile(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.DefaultExt = ".xml";
			ofd.Filter = "HiP-HOPS or ODE XML file (.xml)|*.xml|Dymodia project file (.uproj)|*.uproj|Standalone state machine file (.json)|*.json";
			ofd.InitialDirectory = Directory.GetCurrentDirectory();
			ofd.ShowDialog();

			filename.Text = ofd.FileName;

			// Set new cwd
			try
			{
				Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(ofd.FileName));
			}
			catch { }

			importedModel = CheckTypeAndImport(filename.Text);
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_SelectResultsFile(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.Filter = "HiP-HOPS results file|*.xml";
			ofd.InitialDirectory = Directory.GetCurrentDirectory();
			ofd.ShowDialog();

			resultsFilename.Text = ofd.FileName;

			// Set new cwd
			try
			{
				Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(ofd.FileName));
			}
			catch { }

			// Here we import the results and merge with the first model
			MergeWithResults(resultsFilename.Text);
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_ConvertModel(object sender, RoutedEventArgs e)
		{
			// Convert and return to the main window for merging with parent model (if any)
			if (importedModel == null ||
				importedModel.Type == ConverterModel.ModelType.ODE) // No need to convert if it's already an ODE model
			{
				this.Close();
				return;
			}

			Converter converter = null;

			// Create the appropriate converter
			if (importedModel.Type == ConverterModel.ModelType.HIPHOPS)
			{
				converter = new ConverterHIPtoODE();
			}
			else if (importedModel.Type == ConverterModel.ModelType.DYMODIA)
			{
				converter = new ConverterDYMtoODE();
			}

			if (converter != null)
			{
				var odeModel = converter.Convert(importedModel) as ODELib.ode.Model;

				statusBarText.Text = "Model converted OK";

				// Reset as imported model
				importedModel = odeModel;
			}

			this.Close();
		}

		//----------------------------------------------------------------------------------------------------//

		private ConverterModel.ModelType DetectModelType(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename)) return ConverterModel.ModelType.UNKNOWN; // Abort

			// If it's a .uproj, it's Dymodia
			if (filename.ToLower().EndsWith(".uproj"))
			{
				detectedType.Text = "Dymodia Model";
				return ConverterModel.ModelType.DYMODIA;
			}

			if (filename.ToLower().EndsWith(".json"))
			{
				detectedType.Text = "JSON State Machine";
				return ConverterModel.ModelType.JSON_SM;
			}

			// An XML file could be anything, so we'll need to peek inside
			using (var fileStream = File.Open(filename, FileMode.Open))
			using (var stringStream = new StreamReader(fileStream))
			{
				//string xml = stringStream.ReadToEnd();
				var xmlFile = XDocument.Load(stringStream);

				// ODE files have a Model as root then (probably) SystemElements
				if (xmlFile.Root.Name == "Model" &&
					xmlFile.Root.Elements("SystemElements").Any())
				{
					detectedType.Text = "ODE Model";
					return ConverterModel.ModelType.ODE;
				}

				// HiP-HOPS results files have a HiPHOPS_Result root
				if (xmlFile.Root.Name == "HiP-HOPS_Results")
				{
					detectedType.Text = "HiP-HOPS Result Model";
					return ConverterModel.ModelType.HIPHOPS;
				}

				// HiP-HOPS system files have a Model root, then a Version and ToolVersion inside that
				if (xmlFile.Root.Name == "Model" &&
					xmlFile.Root.Elements("Version").Any() &&
					xmlFile.Root.Elements("ToolVersion").Any())
				{
					detectedType.Text = "HiP-HOPS Model";
					return ConverterModel.ModelType.HIPHOPS;
				}
			}

			return ConverterModel.ModelType.UNKNOWN;
		}

		//----------------------------------------------------------------------------------------------------//

		private ConverterModel CheckTypeAndImport(string filename)
		{
			var type = DetectModelType(filename);
			if (type == ConverterModel.ModelType.UNKNOWN)
			{
				this.statusBarText.Text = $"Could not load model -- invalid file '{filename}'";
				return null;
			}

			switch (type)
			{
				case ConverterModel.ModelType.HIPHOPS:
					return ImportHiPHOPS(filename);
				case ConverterModel.ModelType.ODE:
					return ImportODE(filename);
				case ConverterModel.ModelType.DYMODIA:
					return ImportDYM(filename);
				case ConverterModel.ModelType.JSON_SM:
					return ImportJSON(filename);
			}

			return null;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Imports a HH model
		/// </summary>
		/// <param name="filename">The filename.</param>
		private ODELib.hip.Model ImportHiPHOPS(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename)) return null; // Abort


			using (var fileStream = File.Open(filename, FileMode.Open))
			using (var stringStream = new StreamReader(fileStream))
			{
				ODELib.hip.Model model = null;
				string xml = stringStream.ReadToEnd();
				var serializer = new XmlSerializer(typeof(ODELib.hip.Model));

				// Fix empty tags (e.g. <tag></tag> to <tag/>
				string cleanerRegex = @"<\w+>\s*</\w+>";
				//var matches = Regex.Matches(xml, cleanerRegex; // For debugging
				string cleanXml = Regex.Replace(xml, cleanerRegex, new MatchEvaluator(x => ""));
				//cleanXml = Regex.Replace(xml, @"<[a-zA-Z].[^(><.)]+/>", new MatchEvaluator(x => "")); // For empty tags (e.g. <tag/>

				using (var memoryStream = new MemoryStream((new UTF8Encoding()).GetBytes(cleanXml)))
				{
					model = (ODELib.hip.Model)serializer.Deserialize(memoryStream);
				}
				statusBarText.Text = $"HiP-HOPS model '{filename}' loaded OK";
				importedModel = model;

				// Enable/disable buttons
				this.openResultsFile.IsEnabled = true;
				this.convertModel.Content = "Convert";
				this.convertModel.IsEnabled = true;

				// Initialise and build VM
				model.Initialise();
				importedHIPModelVM = new Viewmodels.hip.ModelVM(model);
				modelHierarchy.ItemsSource = new List<Viewmodels.hip.ModelVM>() { importedHIPModelVM };
				importedPropertyGrid.SelectedObject = importedHIPModelVM;
				return model;
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Imports an ODE model.
		/// </summary>
		/// <param name="filename">The filename.</param>
		private ODELib.ode.Model ImportODE(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename)) return null; // Abort


			using (var fileStream = File.Open(filename, FileMode.Open))
			using (var stringStream = new StreamReader(fileStream))
			{
				ODELib.ode.Model model = null;
				string xml = stringStream.ReadToEnd();
				var serializer = new XmlSerializer(typeof(ODELib.ode.Model));

				using (var memoryStream = new MemoryStream((new UTF8Encoding()).GetBytes(xml)))
				{
					model = (ODELib.ode.Model)serializer.Deserialize(memoryStream);
				}
				statusBarText.Text = $"ODE model '{filename}' loaded OK";
				importedModel = model;

				// Enable/disable buttons
				this.openResultsFile.IsEnabled = false;
				this.convertModel.Content = "Import";
				this.convertModel.IsEnabled = true;

				// Initialise and build VM
				importedODEModelVM = new Viewmodels.ode.ModelVM(model);
				modelHierarchy.ItemsSource = new List<Viewmodels.ode.ModelVM>() { importedODEModelVM };
				importedPropertyGrid.SelectedObject = importedODEModelVM;
				return model;
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Imports a JSON state machine model (as ODE).
		/// </summary>
		/// <param name="filename">The filename.</param>
		private ODELib.ode.Model ImportJSON(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename)) return null; // Abort


			importedModel = StandaloneStateMachineImporter.ImportFromJSON(filename);
			statusBarText.Text = $"JSON state machine '{filename}' loaded OK";

			// Enable/disable buttons
			this.openResultsFile.IsEnabled = false;
			this.convertModel.Content = "Import";
			this.convertModel.IsEnabled = true;

			// Initialise and build VM
			importedODEModelVM = new Viewmodels.ode.ModelVM(importedModel as ODELib.ode.Model);
			modelHierarchy.ItemsSource = new List<Viewmodels.ode.ModelVM>() { importedODEModelVM };
			importedPropertyGrid.SelectedObject = importedODEModelVM;
			return importedModel as ODELib.ode.Model;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Imports a Dymodia model (well, a Dymodia state machine).
		/// </summary>
		/// <param name="filename">The filename.</param>
		private ODELib.dym.Model ImportDYM(string filename)
		{
			if (string.IsNullOrWhiteSpace(filename)) return null; // Abort


			ODELib.dym.Model model = null;

			model = ODELib.dym.Model.Load(filename);

			if (model == null)
			{
				statusBarText.Text = $"ERROR: Dymodia model '{filename}' failed to load";
				return null;
			}
				
			statusBarText.Text = $"Dymodia model '{filename}' loaded OK";
			importedModel = model;

			// Enable/disable buttons
			this.openResultsFile.IsEnabled = false;
			this.convertModel.Content = "Convert";
			this.convertModel.IsEnabled = true;

			// Initialise and build VM
			importedDYMModelVM = new Viewmodels.dym.ModelVM(model);
			modelHierarchy.ItemsSource = new List<Viewmodels.dym.ModelVM>() { importedDYMModelVM };
			importedPropertyGrid.SelectedObject = importedDYMModelVM;
			return model;
		}

		//----------------------------------------------------------------------------------------------------//

		private void MergeWithResults(string filename)
		{
			if (importedModel == null) return;

			ODELib.hip.HipResults results = null;
			using (var fileStream = File.Open(filename, FileMode.Open))
			using (var stringStream = new StreamReader(fileStream))
			{
				var serializer = new XmlSerializer(typeof(ODELib.hip.HipResults));

				string cleanerRegex = @"<\w+>\s*</\w+>";
				string xml = stringStream.ReadToEnd();
				string cleanXml = Regex.Replace(xml, cleanerRegex, new MatchEvaluator(x => ""));
				//string cleanXml = Regex.Replace(xml, @"<[a-zA-Z].[^=(><.)]+/>", new MatchEvaluator(x => ""));
				using (var memoryStream = new MemoryStream((new UTF8Encoding()).GetBytes(cleanXml)))
				{
					results = (ODELib.hip.HipResults)serializer.Deserialize(memoryStream);
				}
			}
			results.ResolveReferences();
			if (importedModel.Type == ConverterModel.ModelType.HIPHOPS)
			{
				ODELib.hip.Model hipModel = (ODELib.hip.Model)importedModel;
				hipModel.MergeWithResults(results);
				statusBarText.Text = "HiP-HOPS Analysis results loaded OK";

				// Rebuild vm
				importedHIPModelVM = new Viewmodels.hip.ModelVM(hipModel);
				modelHierarchy.ItemsSource = new List<Viewmodels.hip.ModelVM>() { importedHIPModelVM };
				importedPropertyGrid.SelectedObject = importedHIPModelVM;
			}
			else
			{
				statusBarText.Text = "HiP-HOPS Analysis results cannot be merged with a non-HiP-HOPS model";
			}

		}

		//----------------------------------------------------------------------------------------------------//

		private void modelHierarchy_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// Set up the binding to the property grid manually since for some reason it doesn't work
			importedPropertyGrid.SelectedObject = e.NewValue;
		}

		//----------------------------------------------------------------------------------------------------//

	}
}
