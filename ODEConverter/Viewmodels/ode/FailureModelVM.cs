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
    public class FailureModelVM
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

        public FailureModelVM(FailureModel fmodel)
        {
            OdeFailureModel = fmodel;

            foreach (var failure in fmodel.Failures)
            {
                var failureVM = new FailureVM(failure);
                Failures.Add(failureVM);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private FailureModel OdeFailureModel { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Name")]
        public string Name { get => OdeFailureModel.Name; set => OdeFailureModel.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Failures")]
        [Description("Failures")]
        [ExpandableObject]
        public ExpandableObservableCollection<FailureVM> Failures { get; private set; } = new ExpandableObservableCollection<FailureVM>();

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
