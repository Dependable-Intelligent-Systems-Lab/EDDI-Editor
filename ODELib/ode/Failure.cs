using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
    public enum FailureOriginType
    {
        Input,
        Output,
        Internal
    }

    public class Failure : Base
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

        public Failure() { }

        public Failure(string name)
        : base(name)
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlAttribute(AttributeName = "originType")]
        public FailureOriginType OriginType { get; set; }
        
        [XmlAttribute(AttributeName = "failureClass")] 
        public string FailureClass { get; set; }
        
        [XmlAttribute(AttributeName = "unavailability")]
        public double Unavailability { get; set; }
        
        public ProbDist FailureProbDistribution { get; set; }
        
        [XmlArray]
        public List<Event> Events { get; private set; } = new List<Event>();
        
        public CommonCauseFailure CausedBy { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
