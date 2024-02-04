using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODELib;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;
using ODEConverter.Viewmodels.ode;

namespace ODEConverter.Viewmodels.dym
{
    [DisplayName("Dymodia Model")]
    public class ModelVM
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

        public ModelVM(ODELib.dym.Model model)
        {
            DymModel = model;

            StateMachines.Add(new StateMachineVM(model.StateMachine));
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        /// <summary>
        /// Underlying model class
        /// </summary>
        private ODELib.dym.Model DymModel { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Model name.")]
        public string Name => DymModel.Name;

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("State Machine")]
        [Description("The state machine.")]
        public List<StateMachineVM> StateMachines { get; set; } = new List<StateMachineVM>();

        //----------------------------------------------------------------------------------------------------//

        public bool IsExpanded { get; set; }

        #endregion Properties

        /*****************************************************************************************************/
        /* Functions
        /*****************************************************************************************************/
        #region Functions
        #endregion Functions


    }
}
