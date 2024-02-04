using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    public class CutSetVM
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

        public CutSetVM(CutSet cutset)
        {
            HipCutSet = cutset;

            foreach (var be in cutset.Events)
            {
                if (be.BasicEvent != null && be.BasicEvent.OriginalBasicEvent != null)
                {
                    var bevm = new BasicEventVM(be.BasicEvent.OriginalBasicEvent);
                    Events.Add(bevm);
                }
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private CutSet HipCutSet { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        public double Unavailability { get => HipCutSet.Unavailability; set => HipCutSet.Unavailability = value; }

        //----------------------------------------------------------------------------------------------------//

        [ExpandableObject]
        public ExpandableList Events { get; set; } = new ExpandableList("Events");

        //----------------------------------------------------------------------------------------------------//

        public string Text
        {
            get
            {
                string text = "[";
                if (Events.Count > 0)
                {
                    for (int i = 0; i < Events.Count - 1; i++)
                    {
                        var be = (BasicEventVM)Events[i];
                        text += be.Name + " AND ";
                    }
                    text += ((BasicEventVM)Events.Last()).Name;
                }
                text += "]";
                return text;
            }
        }

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
