using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Microsoft.Win32;
using ODEConverter.Viewmodels.ode;
using ODELib;
using ODELib.ode;


namespace ODEConverter
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		// https://github.com/xceedsoftware/wpftoolkit/wiki/PropertyGrid
		// https://stackoverflow.com/questions/36286530/xceed-wpf-propertygrid-show-item-for-expanded-collection

		private ODELib.ode.Model _model;
		private Viewmodels.ode.ModelVM _modelVM;
		private string _filename;

		//----------------------------------------------------------------------------------------------------//

		public MainWindow()
		{
			InitializeComponent();
		}

		//----------------------------------------------------------------------------------------------------//
		
		private void InitialiseVM()
		{
			_modelVM                    = new ModelVM(_model);
			modelHierarchy.ItemsSource  = new List<ModelVM>() { _modelVM };
			propertyGrid.SelectedObject = _modelVM;
		}

		//----------------------------------------------------------------------------------------------------//

		private void MergeModels(ODELib.ode.Model importedModel)
		{
			// Is there an existing model? If not, just use it as the current one
			if (_model == null)
			{
				_model = importedModel as ODELib.ode.Model;
				InitialiseVM();
				return;
			}

			// Merge the imported model with the current one

			var currentNode = modelHierarchy.SelectedItem;

			if (currentNode is SystemVM systemVM)
			{
				// Add underneath this
				systemVM.AddSubsystem(importedModel.SystemElements.First());
			}
			else if (currentNode is ModelVM modelVM)
			{
				// Add it directly to the top (will also add any hazards)
				modelVM.AddModel(importedModel);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void MergeFailureModels(ODELib.ode.Model importedModel)
		{
			// Is there an existing model? If not, just use it as the current one
			if (_model == null)
			{
				_model = importedModel as ODELib.ode.Model;
				InitialiseVM();
				return;
			}

			// Merge the imported model with the current one
			var currentNode = modelHierarchy.SelectedItem;

			if (currentNode is SystemVM systemVM)
			{
				// Add to the FailureModels underneath this system
				systemVM.AddFailureModels(importedModel.SystemElements.First());
			}
			else if (currentNode is ModelVM modelVM)
			{
				// Add it directly to the first system
				((SystemVM)modelVM.SystemElements.First()).AddFailureModels(importedModel.SystemElements.First());
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void ReplaceSystem(ODELib.ode.Model importedModel)
		{
			object FindParentOf(SystemVM sys, SystemVM current=null)
			{
				if (current == null)
				{
					foreach (SystemVM top in _modelVM.SystemElements)
					{
						if (top == sys)
						{
							// Parent is the model itself
							return _modelVM;
						}

						var result = FindParentOf(sys, top);
						if (result != null)
						{
							return result;
						}
					}
				}
				else
				{
					foreach (SystemVM child in current.Subsystems)
					{
						if (child == sys)
						{
							// Current is the parent
							return current;
						}
						else
						{
							var result = FindParentOf(sys, child);
							if (result != null)
							{
								return result;
							}
						}
					}
				}
				return null;
			}

			// Is there an existing model? If not, quit
			if (_model == null)
			{
				statusBarText.Text = "No model to replace!";
				return;
			}

			// Get the replacement system; if there's a subsystem, use that (as the top is probably just
			// a perspective)
			ODELib.ode.System replacement = null;
			if (importedModel.SystemElements.First().Subsystems.Count > 0)
			{
				replacement = importedModel.SystemElements.First().Subsystems[0];
			}
			else
			{
				replacement = importedModel.SystemElements.First();
			}

			// *Replace* the selected system with the current one; that means getting the parent
			var currentNode = modelHierarchy.SelectedItem;

			if (currentNode is SystemVM systemVM)
			{
				// Get parent
				var parent = FindParentOf(systemVM);
				if (parent == null) return;

				string result = "";
				if (parent is ModelVM)
				{
					result = _modelVM.ReplaceSystem(systemVM, replacement);
				}
				else if (parent is SystemVM parentVM)
				{
					result = parentVM.ReplaceSystem(systemVM, replacement);
				}
				if (result == "" || result == "OK")
				{
					statusBarText.Text = "Replaced system OK";
				}
				else
				{
					statusBarText.Text = "Error when replacing system: " + result;
				}
			}
			else
			{
				statusBarText.Text = "Can only replace selected system models; no such system is selected";
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_New(object sender, RoutedEventArgs e)
		{
			_model = new ODELib.ode.Model();
			InitialiseVM();
			statusBarText.Text = "Empty model created";
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Open(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.DefaultExt = ".xml";
			ofd.Filter = "ODE XML file (.xml)|*.xml|ODE DDI file (.ddi)|*.ddi";
			ofd.InitialDirectory = Directory.GetCurrentDirectory();
			ofd.ShowDialog();

			filename.Text = ofd.FileName;
			_filename = ofd.FileName;

			if (string.IsNullOrWhiteSpace(_filename)) return; // Abort

			// Set new cwd
			Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(_filename));

			// If it's a DDI file, use the special importer
			if (_filename.ToLower().EndsWith(".ddi"))
			{
				ODELib.ode.DDI_Importer importer = new DDI_Importer();
				ODELib.ode.Model model = importer.ImportFromDDI(_filename);
				statusBarText.Text = $"ODE model '{_filename}' loaded from DDI file OK";
				_model = model;

				// Initialise and build VM
				InitialiseVM();
			}
			else // Just deserialise
			{
				var model = ODELib.ode.Model.LoadFromXML(_filename);
				if (model != null)
				{ 
					statusBarText.Text = $"ODE model '{_filename}' loaded OK";
					_model = model;

					// Initialise and build VM
					InitialiseVM();
				}
				else
				{
					statusBarText.Text = $"Error when loading ODE model from '{_filename}': not loaded";
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Import(object sender, RoutedEventArgs e)
		{
			ImportModel importWindow = new ImportModel();
			importWindow.ShowDialog();

			var importedModel = importWindow.ImportedModel;
			if (importedModel != null)
			{
				if (importedModel.Type != ConverterModel.ModelType.ODE)
				{
					statusBarText.Text = $"Import cancelled";
					return;
				}

				// Replace existing model
				_model = importedModel as ODELib.ode.Model;
				InitialiseVM();
				statusBarText.Text = $"ODE model opened successfully";
			}
			else
			{
				statusBarText.Text = $"Error when importing ODE model";
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Save(object sender, RoutedEventArgs e)
		{
			if (_model == null) return;

			// If we have no current filename yet, use Save As instead
			if (string.IsNullOrEmpty(_filename))
			{
				Click_SaveAs(sender, e);
				return;
			}

			SaveModel();
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_SaveAs(object sender, RoutedEventArgs e)
		{
			if (_model == null) return;

			var sfd = new SaveFileDialog();
			sfd.Filter = "ODE Model file|*.xml"; // Only save as ODE XML; use export for the others
			sfd.InitialDirectory = Directory.GetCurrentDirectory();
			sfd.ShowDialog();

			_filename = sfd.FileName;
			this.filename.Text = _filename;

			// Set new cwd
			try
			{
				Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(_filename));
			}
			catch { }

			SaveModel();
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Export(object sender, RoutedEventArgs e)
		{
			if (_model == null) return;

			var sfd = new SaveFileDialog();
			sfd.Filter = "HiP-HOPS input file|*.xml|HiP-HOPS output file|*.xml|Dymodia project file|*.uproj";
			sfd.InitialDirectory = Directory.GetCurrentDirectory();
			sfd.ShowDialog();

			_filename = sfd.FileName;
			this.filename.Text = _filename;

			// Set new cwd
			try
			{
				Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(_filename));
			}
			catch { }

			MessageBox.Show("Export to other formats still in development", "Cannot export yet", MessageBoxButton.OK, MessageBoxImage.Exclamation);
		}

		//----------------------------------------------------------------------------------------------------//

		private void SaveModel()
		{
			if (_model == null || string.IsNullOrWhiteSpace(_filename)) return;

			var ode_serializer = new XmlSerializer(typeof(ODELib.ode.Model));
			using (var outStream = File.Open(_filename, FileMode.Create))
			{
				ode_serializer.Serialize(outStream, _model);
			}

			statusBarText.Text = "Model saved OK";
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Exit(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Validate(object sender, RoutedEventArgs e)
		{
			bool result = true;
			result = _model.Validate();

			if (result)
			{
				MessageBox.Show("Model is valid", "Valid", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			else
			{
				MessageBox.Show("Model is invalid; see validation.txt for details", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_ImportModelAsSubsystem(object sender, RoutedEventArgs e)
		{
			ImportModel importWindow = new ImportModel();
			importWindow.ShowDialog();

			var importedModel = importWindow.ImportedModel;
			if (importedModel != null)
			{
				if (importedModel.Type != ConverterModel.ModelType.ODE)
				{
					statusBarText.Text = $"Import cancelled";
					return;
				}

				MergeModels(importedModel as ODELib.ode.Model);
				statusBarText.Text = $"ODE model imported successfully";
			}
			else
			{
				statusBarText.Text = $"Error when importing ODE model";
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_ImportFailureModel(object sender, RoutedEventArgs e)
		{
			ImportModel importWindow = new ImportModel();
			importWindow.ShowDialog();

			var importedModel = importWindow.ImportedModel;
			if (importedModel != null)
			{
				if (importedModel.Type != ConverterModel.ModelType.ODE)
				{
					statusBarText.Text = $"Import cancelled";
					return;
				}

				MergeFailureModels(importedModel as ODELib.ode.Model);
				statusBarText.Text = $"ODE model imported successfully";
			}
			else
			{
				statusBarText.Text = $"Error when importing ODE model";
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_ImportModelAndReplace(object sender, RoutedEventArgs e)
		{
			ImportModel importWindow = new ImportModel();
			importWindow.ShowDialog();

			var importedModel = importWindow.ImportedModel;
			if (importedModel != null)
			{
				if (importedModel.Type != ConverterModel.ModelType.ODE)
				{
					statusBarText.Text = $"Import cancelled";
					return;
				}

				ReplaceSystem(importedModel as ODELib.ode.Model);
				statusBarText.Text = $"ODE model imported successfully";
			}
			else
			{
				statusBarText.Text = $"Error when importing ODE model";
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_AddNewEntryAction(object sender, RoutedEventArgs e)
		{
			var newActionWindow = new AddAction();
			newActionWindow.ShowDialog();

			var action = newActionWindow.Action;
			if (action != null)
			{
				// Add it to the current State (if the current item is a state)
				var context = ((MenuItem)sender).DataContext;
				if (context is StateVM stateVM)
				{
					stateVM.AddEntryAction(action);
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_AddNewExitAction(object sender, RoutedEventArgs e)
		{
			var newActionWindow = new AddAction();
			newActionWindow.ShowDialog();

			var action = newActionWindow.Action;
			if (action != null)
			{
				// Add it to the current State (if the current item is a state)
				var context = ((MenuItem)sender).DataContext;
				if (context is StateVM stateVM)
				{
					stateVM.AddExitAction(action);
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_AddNewCauseAction(object sender, RoutedEventArgs e)
		{
			var newActionWindow = new AddAction();
			newActionWindow.ShowDialog();

			var action = newActionWindow.Action;
			if (action != null)
			{
				// Add it to the current State (if the current item is a state)
				var context = ((MenuItem)sender).DataContext;
				if (context is CauseVM causeVM)
				{
					causeVM.AddAction(action);
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_AddNewEvent(object sender, RoutedEventArgs e)
		{
			var newEventWindow = new AddEvent();
			newEventWindow.ShowDialog();

			var @event = newEventWindow.Event;
			if (@event != null)
			{
				// Add it to the current State (if the current item is a state)
				var context = ((MenuItem)sender).DataContext;
				if (context is FailureVM failureVM)
				{
					// And as a short cut, assign the failure itself as the condition if it's a condition event
					if (@event is ConditionEvent conditionEvent)
					{
						conditionEvent.Condition = failureVM.Name;
					}
					failureVM.AddEvent(@event);
				}
				else if (context is TransitionVM transitionVM)
				{
					transitionVM.AddEvent(@event);
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		private void modelHierarchy_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			// Set up the binding to the property grid manually since for some reason it doesn't work
			propertyGrid.SelectedObject = e.NewValue;
		}

		//----------------------------------------------------------------------------------------------------//

	}
}
