using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class Port
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

        public Port()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public string Name { get; set; }
        public string Type { get; set; } // Inport, Outport, Both

        [XmlIgnore]
        public Component ParentComponent { get; set; }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                return ParentComponent.FullName + "." + this.Name;
            }
            set
            {
                Name = value.Split(new char[] { '.' }).Last();
            }
        }

        [XmlIgnore]
        public List<OutputDeviation> OutputDeviations { get; private set; } = new List<OutputDeviation>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        
        public void SetParent(Component parent)
        {
            ParentComponent = parent;
        }

        #endregion Functions

    }
}
