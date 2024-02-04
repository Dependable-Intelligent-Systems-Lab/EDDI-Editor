using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels
{
	/// <summary>
	/// Wrapper class for a generic ObservableCollection that can be expanded as part of a WPF TreeView.
	/// </summary>
	/// <seealso cref="System.Collections.ObjectModel.ObservableCollection&lt;System.Object&gt;" />
	public class ObservableList : ObservableCollection<object>
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

        public ObservableList()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties
        
        public bool IsExpanded { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
