using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class System
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

        public System()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public string Name { get; set; }

        [XmlArray]
        [XmlArrayItem("Component")]
        public List<Component> Components { get; set; } = new List<Component>();
        [XmlArray]
        [XmlArrayItem("Line")]
        public List<Line> Lines { get; set; } = new List<Line>();
        
        [XmlIgnore]
        public Component ParentComponent { get; private set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions

        public void Initialise(Component parent)
        {
            ParentComponent = parent;

            foreach (var component in Components)
            {
                component.Initialise(parent);
            }
        }

        //----------------------------------------------------------------------------------------------------//

        public void MergeWithResults(Component parent, FMEA fmea)
        {
            ParentComponent = parent;

            foreach (var component in Components)
            {
                component.MergeWithResults(parent, fmea);
            }
        }

        #endregion Functions

    }
}
