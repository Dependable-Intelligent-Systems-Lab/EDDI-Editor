using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
    public enum GateType
    {
        OR = 0,
        AND = 1,
        NOT = 2,
        XOR = 3,
        VOTE = 4,
        PAND = 5,
        POR = 6,
        SAND = 7,
        InputEvent = 8,
        OutputEvent = 9
    }

    public class Gate : Cause
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

        public Gate() { }

        public Gate(string name)
            : base(name)
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlAttribute("gateType")]
        public GateType GateType { get; set; }

        // XML Serialisation doesn't always handle inheritance gracefully, so we need to tell the serialiser
        // what element to use for each possible concrete class. It's a bit of a hack, unfortunately.
        [XmlArray]
        [XmlArrayItem(typeof(Cause),ElementName = "Cause")]
        [XmlArrayItem(typeof(Gate),ElementName = "Gate")]
        public List<Cause> Causes { get; private set; } = new List<Cause>(); // i.e., children

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
