using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using ODELib.ode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.ode
{
    public class HazardVM
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

        /// <summary>
        /// Initializes a new instance of the <see cref="HazardVM"/> class.
        /// </summary>
        /// <param name="hazard">The hazard.</param>
        public HazardVM(Hazard hazard)
        {
            OdeHazard = hazard;
            foreach (var failure in hazard.Failures)
            {
                Failures.Add(new FailureVM(failure));
            }
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        private Hazard OdeHazard { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Name")]
        [Description("Name")]
        public string Name { get => OdeHazard.Name; set => OdeHazard.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Hazard Type")]
        [Description("Hazard Type")]
        public HazardType HazardType { get => OdeHazard.HazardType; set => OdeHazard.HazardType = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Condition")]
        [Description("Condition that causes the hazard")]
        public string Condition { get => OdeHazard.Condition; set => OdeHazard.Condition = value; }

        //----------------------------------------------------------------------------------------------------//

        [DisplayName("Failures")]
        [Description("Failures")]
        [ExpandableObject]
        public ExpandableObservableCollection<FailureVM> Failures { get; private set; } = new ExpandableObservableCollection<FailureVM>();

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
