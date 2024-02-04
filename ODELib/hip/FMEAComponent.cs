using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{

    /// <summary>
    /// This is a component that appears in the FMEA results (more of a simple container for events)
    /// </summary>
    public class FMEAComponent
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

        public FMEAComponent()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public string Name { get; set; }
        [XmlArray]
        [XmlArrayItem(typeof(BasicEventResult), ElementName ="BasicEvent")]
        public List<BasicEventResult> Events { get; set; } = new List<BasicEventResult>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
