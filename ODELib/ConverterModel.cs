using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ODELib
{
    /// <summary>
    /// The base model class for all converters. All unique metamodel top-level models should inherit from this.
    /// </summary>
    [DataContract]
    public class ConverterModel
    {
        /*****************************************************************************************************/
        /* Enums/Constants
        /*****************************************************************************************************/
        #region Constants

        public enum ModelType
        {
            ODE,     // An existing ODE model (from XML or DDI)
            HIPHOPS, // HiP-HOPS (both system + results files)
            DYMODIA, // Dymodia (state machines only)
            JSON_SM, // Standalone JSON state machine
            UNKNOWN
        }

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

        public ConverterModel(ModelType type)
        {
            Type = type;
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlIgnore]
        public ModelType Type { get; private set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
