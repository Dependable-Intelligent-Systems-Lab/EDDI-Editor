using ODELib.hip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Basic Event")]
    public class BasicEventVM
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

        public BasicEventVM(BasicEvent be)
        {
            HipBasicEvent = be;

            if (be.UnavailabilityFormula != null)
            {
                UnavailabilityFormula = new UnavailabilityFormulaVM(be.UnavailabilityFormula);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private BasicEvent HipBasicEvent { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Name of the basic event/component failure mode.")]
        public string Name { get => HipBasicEvent.FullName; set => HipBasicEvent.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Unavailability Formula")]
        [Description("The probability distribution of unavailability for the basic event.")]
        [ExpandableObject]
        public UnavailabilityFormulaVM UnavailabilityFormula { get; set; }

        //----------------------------------------------------------------------------------------------------//

        public bool IsExpanded { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
