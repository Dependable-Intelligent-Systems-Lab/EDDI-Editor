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
	/// Interaction logic for AddEvent.xaml
	/// </summary>
	public partial class AddEvent : Window
	{
		// The Event to add (or not)
		public ODELib.ode.Event Event { get; set; } = null;

		//----------------------------------------------------------------------------------------------------//
	
		public AddEvent()
		{
			InitializeComponent();
		}


		//----------------------------------------------------------------------------------------------------//

		private void Click_Add(object sender, RoutedEventArgs e)
		{
			switch (_typeComboBox.Text)
			{
				case "Condition":
					Event = new ODELib.ode.ConditionEvent();
					((ConditionEvent)Event).Condition = _nameBox.Text;
					break;
				case "External":
					Event = new ODELib.ode.ExternalEvent();
					break;
				default:
					Event = new ODELib.ode.Event();
					break;
			}
			Event.Name        = _nameBox.Text;
			Event.Description = _descriptionBox.Text;

			this.Close();
		}

		//----------------------------------------------------------------------------------------------------//

		private void Click_Cancel(object sender, RoutedEventArgs e)
		{
			Event = null;
			this.Close();
		}
	}
}
