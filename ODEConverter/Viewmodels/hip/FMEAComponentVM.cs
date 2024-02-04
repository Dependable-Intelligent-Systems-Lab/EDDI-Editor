using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    public class FMEAComponentVM
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

        public FMEAComponentVM(FMEAComponent comp)
        {
            HipComponent = comp;

            foreach (var be in comp.Events)
            {

            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private FMEAComponent HipComponent { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        public string Name { get => HipComponent.Name; set => HipComponent.Name = value; }
        
        //----------------------------------------------------------------------------------------------------//
        
        [ExpandableObject]
        public ExpandableList Events { get; set; } = new ExpandableList("Events");


        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
