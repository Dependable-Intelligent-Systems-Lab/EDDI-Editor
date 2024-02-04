using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class FMEA : HipResultBase
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

        public FMEA()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        // NB: The components are direct children of the FMEA, so we have to use XmlElement not XmlArray (or XmlArrayItem)
        // https://stackoverflow.com/questions/62884134/how-to-deserialize-child-elements-of-different-types-into-list-collection-of-bas

        [XmlElement(typeof(FMEAComponent), ElementName ="Component")]
        public List<FMEAComponent> Components { get; set; } = new List<FMEAComponent>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions

        #endregion Functions

    }
}
