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
    [DisplayName("HiP-HOPS Component")]
    public class ComponentVM
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

        public ComponentVM(ODELib.hip.Component component)
        {
            HipComponent = component;

            foreach (var port in component.Ports)
            {
                var vm = new PortVM(port);
                Ports.Add(vm);
            }

            foreach (var impl in component.Implementations)
            {
                var vm = new ImplementationVM(impl);
                Implementations.Add(vm);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private ODELib.hip.Component HipComponent { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//


        //[Category("Model Hierarchy")]
        [DisplayName("Implementations")]
        [Description("The possible implementations of this component (limited to just one for now).")] 
        [ExpandableObject]
        public ExpandableObservableCollection<ImplementationVM> Implementations { get; private set; } = new ExpandableObservableCollection<ImplementationVM>();

        //----------------------------------------------------------------------------------------------------//

        //[Category("Analysis Properties")]
        [DisplayName("Include In Optimisation?")]
        [Description("Whether or not to include this component in optimisation.")]
        public bool IncludeInOptimisation { get => HipComponent.IncludeInOptimisation; set => HipComponent.IncludeInOptimisation = value; }

        //----------------------------------------------------------------------------------------------------//


        //[Category("Component Properties")]
        [DisplayName("Name")]
        [Description("The name of the component.")] 
        public string Name { get => HipComponent.Name; set => HipComponent.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Analysis Properties")]
        [DisplayName("Risk Time")]
        [Description("The operating life time (time at risk) of this component, if different from the overall system.")] 
        public double RiskTime { get => HipComponent.RiskTimeValue; set => HipComponent.RiskTime = value.ToString(); }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Model Hierarchy")]
        [DisplayName("Ports")]
        [Description("The ports that serve as the interface for this component.")]
        [ExpandableObject]
        public ExpandableObservableCollection<PortVM> Ports { get; private set; } = new ExpandableObservableCollection<PortVM>();

        //----------------------------------------------------------------------------------------------------//

        //[Category("Component Properties")]
        [DisplayName("Type")]
        [Description("The type of the component.")] 
        public string Type { get => HipComponent.Type; set => HipComponent.Type = value; }

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
