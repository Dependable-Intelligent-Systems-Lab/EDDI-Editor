using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ODELib;
using ODELib.ode;

namespace ODEConverter
{
	/// <summary>
	/// Interaction logic for AddAction.xaml
	/// </summary>
	public partial class AddAction : Window
	{
		// The action to add (or not)
		public ODELib.ode.Action Action { get; set; } = null;

		//----------------------------------------------------------------------------------------------------//
	
		public AddAction()
		{
			InitializeComponent();
		}


		//----------------------------------------------------------------------------------------------------//

		private void Click_Add(object sender, RoutedEventArgs e)
		{
			switch (_typeComboBox.Text)
			{
				case "Message":
					Action = new ODELib.ode.MessageAction();
					((MessageAction)Action).Message = _nameBox.Text;
					break;
				case "Function":
					Action = new ODELib.ode.FunctionAction();
					((FunctionAction)Action).Function = _nameBox.Text;
					break;
				case "Warning":
					Action = new ODELib.ode.WarningAction();
					((WarningAction)Action).Warning = _nameBox.Text;
					break;
				default:
					Action = new ODELib.ode.Action();
					break;
			}
			Action.Name        = _nameBox.Text;
			Action.Description = _descriptionBox.Text;

			this.Close();
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Cancel(object sender, RoutedEventArgs e)
		{
			Action = null;
			this.Close();
		}
	}
}
