using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.hip
{
    public class FailureData
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

        public FailureData()
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public double ComponentFailureRate { get; set; }
        [XmlArray]
        [XmlArrayItem("BasicEvent")]
        public List<BasicEvent> BasicEvents { get; private set; } = new List<BasicEvent>();
        [XmlArray]
        [XmlArrayItem("PotentialCCF")]
        public List<PotentialCCF> PotentialCCFs { get; private set; } = new List<PotentialCCF>();
        [XmlArray]
        [XmlArrayItem("OutputDeviation")]
        public List<OutputDeviation> OutputDeviations { get; private set; } = new List<OutputDeviation>();
        [XmlArray]
        [XmlArrayItem("ExportedPropagation")]
        public List<ExportedPropagation> ExportedPropagations { get; private set; } = new List<ExportedPropagation>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions

        public void Initialise(Component parent)
        {
            foreach (var basicEvent in BasicEvents)
            {
                basicEvent.ParentComponent = parent;

                // Init unavailability formula
                if (basicEvent.UnavailabilityFormula != null)
                {
                    basicEvent.UnavailabilityFormula.Initialise(basicEvent);
                }
            }

            foreach (var odevn in OutputDeviations)
            {
                foreach (var port in parent.Ports)
                {
                    if (port.Name == odevn.PortName)
                    {
                        odevn.SetParent(port);
                        port.OutputDeviations.Add(odevn);
                        break;
                    }
                }
            }
        }

        //----------------------------------------------------------------------------------------------------//

        public void MergeWithResults(Component parent, FMEA fmea)
        {
            foreach (var be in BasicEvents)
            {
                be.MergeWithResults(parent, fmea);
            }
            foreach (var pccf in PotentialCCFs)
            {
                //pccf.MergeWithResults(parent, fmea);
            }
        }

        #endregion Functions

    }
}
