using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using ODELib.hip;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Component Implementation")]
    public class ImplementationVM
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

        public ImplementationVM(Implementation imp)
        {
            HipImp = imp;

            if (imp.System != null)
            {
                System = new SystemVM(imp.System);
            }

            if (imp.FailureData != null)
            {
                FailureData = new FailureDataVM(imp.FailureData);
                Items.Add(FailureData.Data);
            }

            if (System != null)
            {
                Subsystem.Add(System);
                Items.Add(Subsystem);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Implementation HipImp { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//


        //[Category("Component Properties")]
        [DisplayName("Name")]
        [Description("The name of the implementation.")]
        public string Name { get => HipImp.Name; set => HipImp.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Component Properties")]
        [DisplayName("Exclude from optimisation")]
        [Description("Whether or not to exclude from optimisation.")]
        public bool ExcludeFromOptimisation { get => HipImp.ExcludeFromOptimisation; set => HipImp.ExcludeFromOptimisation = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Component Properties")]
        [DisplayName("Cost")]
        [Description("The cost of the component implementation.")]
        public string Cost { get => HipImp.Cost; set => HipImp.Cost = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Component Properties")]
        [DisplayName("Weight")]
        [Description("The weight of the component implementation.")]
        public string Weight { get => HipImp.Weight; set => HipImp.Weight = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Analysis Properties")]
        [DisplayName("Block Type")]
        [Description("The block type of this implementation. This decides whether analysis goes to the subsystem only (Refined), this level only (Not Refined), or both.")]
        public HBlockType HBlockType { get => HipImp.HBlockType; set => HipImp.HBlockType = value; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Analysis Properties")]
        [DisplayName("Failure Data")]
        [Description("The failure data for this implementation.")]
        [ExpandableObject]
        public FailureDataVM FailureData { get; private set; }

        //----------------------------------------------------------------------------------------------------//

        //[Category("Model Hierarchy")]
        [DisplayName("Subsystem")]
        [Description("The subsystem, if any.")]
        [ExpandableObject]
        public SystemVM System { get; private set; }

        //----------------------------------------------------------------------------------------------------//

        public ExpandableList Subsystem { get; private set; } = new ExpandableList("Subsystem");

        //----------------------------------------------------------------------------------------------------//

        public ObservableList Items { get; private set; } = new Viewmodels.ObservableList();

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
