using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODELib;
using Extended.Wpf.Toolkit.PropertyGrid.Collection;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.ComponentModel;

namespace ODEConverter.Viewmodels.ode
{
	[DisplayName("ODE Model")]
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

		public ModelVM(ODELib.ode.Model model)
		{
			OdeModel = model;

			foreach (var system in OdeModel.SystemElements)
			{
				SystemElements.Add(new SystemVM(system));
			}

			foreach (var hazard in OdeModel.Hazards)
			{
				Hazards.Add(new HazardVM(hazard));
			}

			Items.Add(SystemElements);
			Items.Add(Hazards);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		/// <summary>
		/// Underlying model class
		/// </summary>
		private ODELib.ode.Model OdeModel { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Model name.")]
		public string Name => OdeModel.SystemElements.FirstOrDefault()?.Name ?? "Unnamed System";

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Hazards")]
		[Description("The top-level systems failures or hazards.")]
		[ExpandableObject]
		public ExpandableList Hazards { get; private set; } = new ExpandableList("Hazards");

		//----------------------------------------------------------------------------------------------------//

		public ObservableList Items { get; set; } = new ObservableList();

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("System Elements")]
		[Description("The top-level systems/components.")]
		[ExpandableObject]
		public ExpandableList SystemElements { get; private set; } = new ExpandableList("System Elements");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public void AddModel(ODELib.ode.Model model)
		{
			if (model == OdeModel) return;

			foreach (var sys in model.SystemElements)
			{
				OdeModel.SystemElements.Add(sys);
				SystemElements.Add(new SystemVM(sys));
			}

			foreach (var hazard in model.Hazards)
			{
				OdeModel.Hazards.Add(hazard);
				Hazards.Add(new HazardVM(hazard));
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Replaces a (dummy) system with a newly imported one.
		/// The idea is that if you have a placeholder system, you can import a model to replace it as a full
		/// hierarchy/subsystem.
		/// </summary>
		/// <param name="existing">The existing.</param>
		/// <param name="replacement">The replacement.</param>
		/// <returns></returns>
		public string ReplaceSystem(SystemVM existing, ODELib.ode.System replacement)
		{
			if (!SystemElements.Contains(existing))
			{
				return $"Could not find system '{existing.Name}' to replace.";
			}

			SystemElements.Remove(existing);
			OdeModel.SystemElements.Remove(existing.OdeSystem);
			SystemElements.Add(new SystemVM(replacement));
			OdeModel.SystemElements.Add(replacement);
			return "OK";
		}

		//----------------------------------------------------------------------------------------------------//

		#endregion Functions


	}
}
