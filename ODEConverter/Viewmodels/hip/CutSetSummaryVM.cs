using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.hip
{
    public class CutSetSummaryVM
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

        public CutSetSummaryVM(CutSetsSummary css)
        {
            HipSummary = css;
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private CutSetsSummary HipSummary { get; set; }

        /*****************************************************************************************************/
        public string Text => Number + " cutsets of order " + Order;
        public int Order { get => HipSummary.Order; set => HipSummary.Order = value; }
        public int Number { get => HipSummary.Number; set => HipSummary.Number = value; }


        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
