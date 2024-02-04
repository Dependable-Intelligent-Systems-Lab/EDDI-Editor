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
    [DisplayName("HiP-HOPS Component Failure Data")]
    public class FailureDataVM
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

        public FailureDataVM(FailureData fdata)
        {
            HipFailureData = fdata;

            foreach (var be in fdata.BasicEvents)
            {
                var vm = new BasicEventVM(be);
                BasicEvents.Add(vm);
            }

            foreach (var pccf in fdata.PotentialCCFs)
            {
                var vm = new PotentialCCFVM(pccf);
                PotentialCCFs.Add(vm);
            }

            foreach (var odevn in fdata.OutputDeviations)
            {
                var vm = new OutputDeviationVM(odevn);
                OutputDeviations.Add(vm);
            }

            Data.Add(BasicEvents);
            Data.Add(PotentialCCFs);
            Data.Add(OutputDeviations);
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private FailureData HipFailureData { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Basic Events")]
        [Description("The list of basic events, i.e., component-level failures.")]
        [ExpandableObject]
        public ExpandableList Data { get; private set; } = new ExpandableList("Failure Data");

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Component Failure Rate")]
        [Description("The general failure rate of this component, if applicable")]
        private double ComponentFailureRate { get => HipFailureData.ComponentFailureRate; set => HipFailureData.ComponentFailureRate = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Basic Events")]
        [Description("The list of basic events, i.e., component-level failures.")]
        [ExpandableObject]
        public ExpandableList BasicEvents { get; private set; } = new ExpandableList("Basic Events");

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Potential CCFs")]
        [Description("The list of potential common cause failures.")]
        [ExpandableObject]
        public ExpandableList PotentialCCFs { get; private set; } = new ExpandableList("Potential CCFs");

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Output Deviations")]
        [Description("The list of output deviations of this component.")]
        [ExpandableObject]
        public ExpandableList OutputDeviations { get; private set; } = new ExpandableList("Output Deviations");

        //----------------------------------------------------------------------------------------------------//

        //[DisplayName("Exported Propagations")]
        //[Description("The exported propagations of this component.")]
        //[ExpandableObject]
        //public ExpandableObservableCollection<ExportedPropagation> ExportedPropagations { get; private set; } = new ExpandableObservableCollection<ExportedPropagation>();

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
