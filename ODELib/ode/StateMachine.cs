using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ODELib.ode
{
    public class StateMachine : FailureModel
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

        public StateMachine() { }
        public StateMachine(string name)
            : base(name)
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        [XmlArray]
        public List<State> States { get; private set; } = new List<State>();

        [XmlArray]
        public List<Transition> Transitions { get; private set; } = new List<Transition>();
        //public List<TransitionMatrix> TransitionMatrices { get; private set; } = new List<TransitionMatrix>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
