using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
    public class SignalVM
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

        public SignalVM(Signal signal)
        {
            OdeSignal = signal;

            if (signal.FromPort != null)
            {
                FromPort = new PortVM(signal.FromPort);
            }
            if (signal.ToPort != null)
            {
                ToPort = new PortVM(signal.ToPort);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Signal OdeSignal { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Name")]
        public string Name { get => string.IsNullOrWhiteSpace(OdeSignal.Name) ? "Unnamed Signal" : OdeSignal.Name; set => OdeSignal.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("From Port")]
        [Description("Source port of this signal")]
        public PortVM FromPort { get; set; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("To Port")]
        [Description("Destination port of this signal")]
        public PortVM ToPort { get; set; }

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
