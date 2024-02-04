using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    /// <summary>
    /// Basic event from the results file; simliar to the normal one but with more fields
    /// </summary>
    public class BasicEventResult
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

        public BasicEventResult()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlAttribute]
        public string ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Description { get; set; }
        public double Unavailability { get; set; }
        public double Frequency { get; set; }
        [XmlArray]
        [XmlArrayItem("Effect")]
        public List<Effect> Effects { get; set; } = new List<Effect>();

        [XmlIgnore]
        public BasicEvent OriginalBasicEvent { get; set; }
        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
