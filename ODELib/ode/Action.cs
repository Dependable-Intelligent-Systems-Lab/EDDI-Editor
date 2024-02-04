using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ODELib.ode
{
    public class Action : Base
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
        
        public Action() { }

        public Action(string name) 
            : base(name)
        {
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        public List<Action> Prerequisites { get; private set; } = new List<Action>();
        public List<Action> SubActions { get; private set; } = new List<Action>();
        public List<Event> TriggeringEvents { get; private set; } = new List<Event>();

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions

    }
}
