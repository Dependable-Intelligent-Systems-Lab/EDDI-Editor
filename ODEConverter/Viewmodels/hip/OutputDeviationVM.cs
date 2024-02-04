using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.hip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Output Deviation")]
    public class OutputDeviationVM
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

        public OutputDeviationVM(OutputDeviation odevn)
        {
            HipODEVN = odevn;

        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private OutputDeviation HipODEVN { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Name of the output deviation, including failure class and port.")]
        public string Name { get => HipODEVN.Name; set => HipODEVN.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Severity")]
        [Description("The severity rating of the output deviation.")]
        public double Severity 
        {
            get
            {
                if (double.TryParse(HipODEVN.Severity, out double d))
                {
                    return d;
                }
                return 0;
            }

            set => HipODEVN.Severity = value.ToString(); 
        }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Is System Outport?")]
        [Description("Whether or not this output deviation is a system-level failure.")]
        public bool SystemOutport { get => HipODEVN.SystemOutport; set => HipODEVN.SystemOutport = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Failure logic")]
        [Description("The causes of the output deviation.")]
        public string Cause { get => HipODEVN.Causes.First()?.FailureLogic; set { if (HipODEVN.Causes.Count() > 0) HipODEVN.Causes.First().FailureLogic = value; } }

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
