using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ODELib.hip;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Hazard")]
    public class HazardVM
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

        public HazardVM(Hazard hazard)
        {
            HipHazard = hazard;

        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Hazard HipHazard { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Description")]
        [Description("The description of the hazard.")] 
        public string Description { get => HipHazard.Description; set => HipHazard.Description = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("The name of the hazard.")] 
        public string Name { get => HipHazard.Name; set => HipHazard.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Safety Integrity Level")]
        [Description("The required integrity level (SIL, ASIL, DAL etc) associated with the hazard.")] 
        public int SafetyRequirement { get => HipHazard.SafetyRequirementValue; set => HipHazard.SafetyRequirement = value.ToString(); }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Cause")]
        [Description("The cause of the hazard.")]
        public string Cause 
        { 
            get
            {
                if (HipHazard.Causes.Count > 0)
                {
                    return HipHazard.Causes.First().FailureLogic;
                }
                return "";
            }
            set
            {
                if (HipHazard.Causes.Count > 0)
                {
                    HipHazard.Causes.First().FailureLogic = value;
                }
            }
        }

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
