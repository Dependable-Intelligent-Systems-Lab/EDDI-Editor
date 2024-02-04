using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.hip;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Port")]
    public class PortVM
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

        public PortVM(Port port)
        {
            Port = port;

            foreach (var odevn in port.OutputDeviations)
            {
                var odevnVM = new OutputDeviationVM(odevn);
                OutputDeviations.Add(odevnVM);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Port Port { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("The name of the port.")]
        public string Name { get => Port.Name; set => Port.Name = value; }

        //----------------------------------------------------------------------------------------------------//


        [DisplayName("Type")]
        [Description("The type of the port -- usually Inport, Outport, or Both, but customisable.")]
        public string Type { get => Port.Type; set => Port.Type = value; } // Inport, Outport, Both


        //----------------------------------------------------------------------------------------------------//


        [DisplayName("Output Deviations")]
        [Description("Deviations of output from this port.")]
        [ExpandableObject]
        public ExpandableObservableCollection<OutputDeviationVM> OutputDeviations { get; set; } = new ExpandableObservableCollection<OutputDeviationVM>();

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
