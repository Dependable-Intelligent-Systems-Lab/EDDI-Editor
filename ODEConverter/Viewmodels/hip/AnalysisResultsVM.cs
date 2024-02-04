using ODELib.hip;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    public class AnalysisResultsVM
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

        public AnalysisResultsVM(HipResults result)
        {
            FMEA = new FMEAVM(result.FMEA);

            foreach (var ft in result.FaultTrees)
            {
                var ftvm = new FaultTreeVM(ft);
                FaultTrees.Add(ftvm);
            }

            Items.Add(FMEA);
            foreach (var ft in FaultTrees)
            {
                Items.Add(ft);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [ExpandableObject]
        public FMEAVM FMEA { get; set; }

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public ExpandableList Items { get; set; } = new ExpandableList("Results");

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public ExpandableObservableCollection<FaultTreeVM> FaultTrees { get; set; } = new ExpandableObservableCollection<FaultTreeVM>();

        public bool IsExpanded { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
