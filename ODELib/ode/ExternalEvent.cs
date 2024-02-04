using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
    public class ExternalEvent : Event
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

        public ExternalEvent() { }
        public ExternalEvent(string name)
            : base(name)
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlAttribute("sourceID")]
        public string SourceID { get; set; }
        
        [XmlAttribute("message")]
        public string Message { get; set; }

        [XmlAttribute("messageType")]
        public string MessageType { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
