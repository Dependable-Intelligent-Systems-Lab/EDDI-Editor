using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace ODEConverter
{
	/// <summary>
	/// Interaction logic for ImportSystem.xaml
	/// </summary>
	public partial class ImportSystem : Window
	{
		public ImportSystem()
		{
			InitializeComponent();
		}

		private void click_Open(object sender, RoutedEventArgs e)
		{

		}

		private void click_Cancel(object sender, RoutedEventArgs e)
		{

		}

		private void click_SelectResultsFile(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.DefaultExt = ".xml";
			ofd.Filter = "HiP-HOPS or ODE XML file (.xml)|*.xml|Dymodia project file (.uproj)|*.uproj";
			ofd.InitialDirectory = Directory.GetCurrentDirectory();
			ofd.ShowDialog();

			resultsFilename.Text = ofd.FileName;
		}

		private void click_SelectSystemFile(object sender, RoutedEventArgs e)
		{
			var ofd = new OpenFileDialog();
			ofd.DefaultExt = ".xml";
			ofd.Filter = "HiP-HOPS or ODE XML file (.xml)|*.xml|Dymodia project file (.uproj)|*.uproj";
			ofd.InitialDirectory = Directory.GetCurrentDirectory();
			ofd.ShowDialog();

			systemFilename.Text = ofd.FileName;
		}
	}
}
