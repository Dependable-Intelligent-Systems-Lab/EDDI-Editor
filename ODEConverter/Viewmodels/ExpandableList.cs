using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels
{
	/// <summary>
	/// Wrapper class for an ExpandableObservableCollection that can be expanded as part of a WPF TreeView.
	/// </summary>
	/// <seealso cref="Extended.Wpf.Toolkit.PropertyGrid.Collection.ExpandableObservableCollection&lt;System.Object&gt;" />
	[ExpandableObject]
	public class ExpandableList : ExpandableObservableCollection<object>
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

		public ExpandableList(string name="")
		{
			Name = name;
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties
		
		public string Name { get; set; }
		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions
		#endregion Functions

	}
}
