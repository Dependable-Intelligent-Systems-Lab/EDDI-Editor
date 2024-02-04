using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class Hazard
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

        public Hazard()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public string Name { get; set; }
        public string Description { get; set; }
        public string SafetyRequirement { get; set; }
        [XmlIgnore]
        public int SafetyRequirementValue
        {
            get
            {
                if (int.TryParse(SafetyRequirement, out int val))
                {
                    return val;
                }
                return 0;
            }
        }
        
        [XmlArray]
        [XmlArrayItem("Cause")]
        public List<Cause> Causes { get; private set; } = new List<Cause>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
