using ODELib.hip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Failure Cause")]
    public class CauseVM
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
        
        public CauseVM() 
        {
            // User tried to make a new cause; we need to add the underlying cause too
            HipCause = new Cause();
        }

        //----------------------------------------------------------------------------------------------------//

        public CauseVM(Cause cause)
        {
            HipCause = cause;
        }


        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Cause HipCause { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//


        //[Category("Model Hierarchy")]
        [DisplayName("Failure logic")]
        [Description("The logic describing the cause of the hazard or failure.")]
        public string FailureLogic { get => HipCause?.FailureLogic; set => HipCause.FailureLogic = value; }


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
