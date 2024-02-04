using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODELib;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;

namespace ODEConverter.Viewmodels.hip
{
    [DisplayName("HiP-HOPS Model")]
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

        public ModelVM(ODELib.hip.Model model)
        {
            HipModel = model;

            foreach (var p in model.Perspectives)
            {
                var vm = new PerspectiveVM(p);
                Perspectives.Add(vm);
            }

            foreach (var h in model.Hazards)
            {
                var vm = new HazardVM(h);
                Hazards.Add(vm);
            }

            if (model.Results != null)
            {
                AnalysisResults = new AnalysisResultsVM(model.Results);
            }

            Items.Add(Hazards);
            Items.Add(Perspectives);
            Items.Add(new ExpandableList("Results") { AnalysisResults });
        }

        #endregion Constructors

        /*****************************************************************************************************/
        /* Properties
        /*****************************************************************************************************/
        #region Properties

        /// <summary>
        /// Underlying model class
        /// </summary>
        public ODELib.hip.Model HipModel { get; set; }

        //----------------------------------------------------------------------------------------------------//
        //----------------------------------------------------------------------------------------------------//

        public ObservableList Items { get; set; } = new ObservableList();

        //----------------------------------------------------------------------------------------------------//

        [Category("Analysis Properties")]
        [DisplayName("Analysis Results")]
        [Description("The FMEA and FTA results.")]
        public AnalysisResultsVM AnalysisResults { get; set; }

        //----------------------------------------------------------------------------------------------------//

        [Category("Model Properties")]
        [DisplayName("Model description")]
        [Description("The description of the model.")]
        public string Description { get => HipModel.Description; set => HipModel.Description = value; }

        //----------------------------------------------------------------------------------------------------//

        [Category("Model Hierarchy")]
        [DisplayName("Hazards")]
        [Description("The top-level hazards of the system.")]
        [ExpandableObject]
        public ExpandableList Hazards { get; private set; } = new ExpandableList("Hazards");

        //----------------------------------------------------------------------------------------------------//

        [Category("Model Properties")]
        [DisplayName("Model Name")]
        [Description("The name of the model.")]
        public string Name { get => HipModel.Name; set => HipModel.Name = value; }

        //----------------------------------------------------------------------------------------------------//

        [Category("Model Hierarchy")]
        [DisplayName("Perspectives")]
        [Description("The set of perspectives for this model, e.g. design architecture, hardware architecture, software architecture etc.")]
        [ExpandableObject]
        public ExpandableList Perspectives { get; private set; } = new ExpandableList("Perspectives");

        //----------------------------------------------------------------------------------------------------//
        
        [Category("Analysis Properties")]
        [DisplayName("Risk Time")]
        [Description("The mission lifetime of the system.")]
        public double RiskTime { get => HipModel.RiskTime; set => HipModel.RiskTime = value; }

        //----------------------------------------------------------------------------------------------------//


        [Category("Analysis Properties")]
        [DisplayName("Maximum Cut Set Size")]
        [Description("The maximum order of cut sets to be generated during FTA.")]
        public int MaxCutSetOrder { get => HipModel.MaxCutSetOrder; set => HipModel.MaxCutSetOrder = value; }

        //----------------------------------------------------------------------------------------------------//

        [Category("Model Properties")]
        [DisplayName("Tool Version Name")]
        [Description("The name of the tool that originally generated this model file.")]
        public string ToolVersionName { get => HipModel.ToolVersion.Name; set => HipModel.ToolVersion = new ODELib.hip.ToolVersion() { Name = value, Number = HipModel.ToolVersion.Number }; }
        
        //----------------------------------------------------------------------------------------------------//
        
        [Category("Model Properties")]
        [DisplayName("Tool Version Number")]
        [Description("The version number of the tool that originally generated this model file.")]
        public string ToolVersionNumber { get => HipModel.ToolVersion.Number; set => HipModel.ToolVersion = new ODELib.hip.ToolVersion() { Name = HipModel.ToolVersion.Name, Number = value }; }

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
