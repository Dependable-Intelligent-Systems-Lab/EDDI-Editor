using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    /// <summary>
    /// Effect of a Basic Event / Failure Mode from an FMEA
    /// </summary>
    public class Effect
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

        public Effect()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlAttribute]
        public string ID { get; set; }
        public string Name { get; set; } // Name of the output deviation caused
        public bool SinglePointFailure { get; set; }

        [XmlIgnore]
        public FaultTree FaultTree { get; set; }

        [XmlIgnore]
        public Hazard Hazard { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
