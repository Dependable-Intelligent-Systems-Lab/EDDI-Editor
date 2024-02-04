using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Perspective")]
    public class PerspectiveVM
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

        public PerspectiveVM(ODELib.hip.Perspective p)
        {
            HipPerspective = p;

            if (p.System != null)
            {
                System = new SystemVM(p.System);
                SystemList.Add(System);
            }
        }


        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties
        private ODELib.hip.Perspective HipPerspective { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//


        [DisplayName("Name")]
        [Description("The name of the perspective.")]
        public string Name { get => HipPerspective.Name; set => HipPerspective.Name = value; }

        //----------------------------------------------------------------------------------------------------//


        [DisplayName("System")]
        [Description("The top-level system in this perspective.")]
        [ExpandableObject]
        public SystemVM System { get; private set; }

        //----------------------------------------------------------------------------------------------------//


        [DisplayName("System")]
        [Description("The top-level system in this perspective.")]
        [ExpandableObject]
        public ExpandableObservableCollection<SystemVM> SystemList { get; private set; } = new ExpandableObservableCollection<SystemVM>();

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
