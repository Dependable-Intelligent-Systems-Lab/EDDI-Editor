using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class CutSet
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

        public CutSet()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public double Unavailability { get; set; }
        public double Frequency { get; set; }
        public double UnavailabilitySort { get; set; }
        [XmlArray]
        [XmlArrayItem("Event")]
        public List<BasicEventRef> Events { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions

        public void ResolveReferences(FMEA fmea)
        {
            foreach (var e in Events)
            {
                e.ResolveReferences(fmea);
            }
        }

        #endregion Functions

    }
}
