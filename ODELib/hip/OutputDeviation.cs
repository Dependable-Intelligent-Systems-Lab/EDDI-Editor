using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class OutputDeviation
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

        public OutputDeviation()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public string Name { get; set; }

        public string Severity { get; set; } // To work around problems with <Severity />, we use a string and store the double separately
        
        [XmlIgnore]
        public double SeverityValue
        {
            get
            {
                if (double.TryParse(Severity, out double val))
                {
                    return val;
                }
                return 0;
            }

        }

        public bool SystemOutport { get; set; }

        [XmlArray]
        [XmlArrayItem("Cause")]
        public List<Cause> Causes { get; private set; } = new List<Cause>();

        [XmlIgnore]
        public Port ParentPort { get; set; }

        [XmlIgnore]
        public string PortName
        {
            get
            {
                return Name.Split(new char[] { '-' }).Last();
            }
        }

        [XmlIgnore]
        public string FailureClass
        {
            get
            {
                return Name.Split(new char[] { '-' }).First();
            }
        }

        [XmlIgnore]
        public string FullName
        {
            get
            {
                return FailureClass + "-" + ParentPort.FullName;
            }
        }

        [XmlIgnore]
        public List<Hazard> Effects { get; set; } = new List<Hazard>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions

        public void SetParent(Port parent)
        {
            ParentPort = parent;
            if (!parent.OutputDeviations.Contains(this))
            {
                parent.OutputDeviations.Add(this);
            }
        }

        #endregion Functions

    }
}
