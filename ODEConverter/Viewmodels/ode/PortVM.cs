using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.ode;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
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
            OdePort = port;
            foreach (var failure in OdePort.InterfaceFailures)
            {
                var vm = new FailureVM(failure);
                InterfaceFailures.Add(vm);
            }
            foreach (var rport in OdePort.RefinedPorts)
            {
                RefinedPorts.Add(new PortVM(rport));
            }

            if (port.AssuranceLevel != null)
            {
                AssuranceLevel = new AssuranceLevelVM(port.AssuranceLevel);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Port OdePort { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Name")]
        public string Name { get => OdePort.Name; set => OdePort.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Flow Type")]
        [Description("Flow Type")]
        public string FlowType { get => OdePort.FlowType; set => OdePort.FlowType = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Direction")]
        [Description("Direction")]
        public PortDirection Direction { get => OdePort.Direction; set => OdePort.Direction = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Interface Failures")]
        [Description("Interface Failures")]
        [ExpandableObject]
        public ExpandableObservableCollection<FailureVM> InterfaceFailures { get; private set; } = new ExpandableObservableCollection<FailureVM>();

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Refined Ports")]
        [Description("Refined Ports")]
        [ExpandableObject]
        public ExpandableObservableCollection<PortVM> RefinedPorts { get; private set; } = new ExpandableObservableCollection<PortVM>();

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Assurance Level")]
        [Description("Assurance Level")]
        public AssuranceLevelVM AssuranceLevel { get; set; }

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
