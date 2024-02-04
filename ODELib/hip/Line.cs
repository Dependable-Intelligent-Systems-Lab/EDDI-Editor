using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class Line
    {
        /*****************************************************************************************************/
        /* Enums/Constants
        /*****************************************************************************************************/
        #region Constants

        public enum LineDefault
        {
            AND,
            OR,
            OUTPUT
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

        public Line()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public LineDefault Default { get; set; }
        public string Type { get; set; } // Directed or not
        public bool IsDirected => Type == "Directed";
        [XmlArray]
        [XmlArrayItem("Connection")]
        public List<Connection> Connections { get; private set; } = new List<Connection>();


        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }

    public struct Connection
    {
        public string Port;
        public string PortExpression;
    }
}
