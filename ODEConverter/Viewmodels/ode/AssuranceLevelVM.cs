using ODELib.ode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODEConverter.Viewmodels.ode
{
    public class AssuranceLevelVM
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

        public AssuranceLevelVM(ODELib.ode.AssuranceLevel sil)
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private AssuranceLevel OdeAssuranceLevel { get; set; }

        //----------------------------------------------------------------------------------------------------//

        public string Name { get => OdeAssuranceLevel.Name; set => OdeAssuranceLevel.Name = value; }
        
        //----------------------------------------------------------------------------------------------------//

        public string Value { get => OdeAssuranceLevel.Value; set => OdeAssuranceLevel.Value = value; }


        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
