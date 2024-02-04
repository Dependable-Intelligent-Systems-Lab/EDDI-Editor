using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    public class FaultTreeVM
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

        public FaultTreeVM(FaultTree faultTree)
        {
            HipFaultTree = faultTree;

            TopNode = new FaultTreeNodeVM(faultTree.TopNode.First());

            foreach (var cutsetSummary in faultTree.CutSetsSummary)
            {
                CutSetsSummary.Add(new CutSetSummaryVM(cutsetSummary));
            }

            foreach (var cutsetlist in faultTree.AllCutSets)
            {
                foreach (var cutset in cutsetlist.CutSets)
                {
                    var csvm = new CutSetVM(cutset);
                    AllCutSets.Add(csvm);
                }
            }

            if (faultTree.Hazard != null)
            {
                Hazard = new HazardVM(faultTree.Hazard);
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private FaultTree HipFaultTree { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        public string ID { get => HipFaultTree.ID; set => HipFaultTree.ID = value; }

        //----------------------------------------------------------------------------------------------------//

        public string Name { get => HipFaultTree.Name; set => HipFaultTree.Name = value; }

        //----------------------------------------------------------------------------------------------------//
        
        public string Description { get => HipFaultTree.Description; set => HipFaultTree.Description = value; }

        //----------------------------------------------------------------------------------------------------//
        
        public int SIL { get => HipFaultTree.SIL; set => HipFaultTree.SIL = value; }
        
        //----------------------------------------------------------------------------------------------------//
        
        public double Unavailability { get => HipFaultTree.Unavailability; set => HipFaultTree.Unavailability = value; }

        //----------------------------------------------------------------------------------------------------//
        
        public double Severity { get => HipFaultTree.Severity; set => HipFaultTree.Severity = value; }

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public FaultTreeNodeVM TopNode { get; set; }

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public ExpandableList CutSetsSummary { get; set; } = new ExpandableList();

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public ExpandableList AllCutSets { get; set; } = new ExpandableList();

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public HazardVM Hazard { get; set; }

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
