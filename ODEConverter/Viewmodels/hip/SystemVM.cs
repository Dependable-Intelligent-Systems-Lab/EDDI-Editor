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
    [DisplayName("HiP-HOPS System")]
    public class SystemVM
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

        public SystemVM(ODELib.hip.System sys)
        {
            HipSystem = sys;

            foreach (ODELib.hip.Component c in sys.Components)
            {
                var vm = new ComponentVM(c);
                Components.Add(vm);
            }

            foreach (ODELib.hip.Line l in sys.Lines)
            {
                var vm = new LineVM(l);
                Lines.Add(vm);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private ODELib.hip.System HipSystem { get; set; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("System Properties")]
        [DisplayName("Name")]
        [Description("The name of the system.")]
        public string Name { get => HipSystem.Name; set => HipSystem.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Model Hierarchy")]
        [DisplayName("Components")]
        [Description("The components in this system.")]
        [ExpandableObject]
        public ExpandableObservableCollection<ComponentVM> Components { get; private set; } = new ExpandableObservableCollection<ComponentVM>();

        //----------------------------------------------------------------------------------------------------//

        //[Category("Model Hierarchy")]
        [DisplayName("Connections")]
        [Description("The connections between components in this system.")]
        [ExpandableObject]
        public ExpandableObservableCollection<LineVM> Lines { get; private set; } = new ExpandableObservableCollection<LineVM>();

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
