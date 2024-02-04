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
	[DisplayName("State")]
	public class StateVM
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

		public StateVM(ODELib.ode.State state)
		{
			OdeState = state;

			IsFailState = state.IsFailState;
			IsInitialState = state.IsInitialState;

			foreach (var action in state.OnEntry)
			{
				OnEntry.Add(ActionVM.CreateAppropriateActionVM(action));
			}

			foreach (var action in state.OnExit)
			{
				OnExit.Add(ActionVM.CreateAppropriateActionVM(action));
			}

			Items.Add(OnEntry);
			Items.Add(OnExit);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private ODELib.ode.State OdeState { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => OdeState.Name; set => OdeState.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		public ObservableList Items { get; set; } = new ObservableList();

		//----------------------------------------------------------------------------------------------------//


		[DisplayName("IsFailState")]
		[Description("Is Fail State?")]
		public bool IsFailState { get => OdeState.IsFailState; set => OdeState.IsFailState = value; }

		//----------------------------------------------------------------------------------------------------//


		[DisplayName("IsInitialState")]
		[Description("Is Initial State?")]
		public bool IsInitialState { get => OdeState.IsInitialState; set => OdeState.IsInitialState = value; }

		//----------------------------------------------------------------------------------------------------//


		[DisplayName("Entry Actions")]
		[Description("Actions triggered on entry")]
		[ExpandableObject]
		public ExpandableList OnEntry { get; private set; } = new ExpandableList("OnEntry");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Exit Actions")]
		[Description("Actions triggered on exit")]
		[ExpandableObject]
		public ExpandableList OnExit { get; private set; } = new ExpandableList("OnExit");

		//----------------------------------------------------------------------------------------------------//


		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }
		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Adds a new enter Action to the state.
		/// </summary>
		/// <param name="action">The action.</param>
		public void AddEntryAction(ODELib.ode.Action action)
		{
			OnEntry.Add(ActionVM.CreateAppropriateActionVM(action));

			// Works better if you don't forget to add to the model...
			OdeState.OnEntry.Add(action);
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Adds a new exit Action to the state.
		/// </summary>
		/// <param name="action">The action.</param>
		public void AddExitAction(ODELib.ode.Action action)
		{
			OnExit.Add(ActionVM.CreateAppropriateActionVM(action));

			// Also add to the model, d'oh...
			OdeState.OnExit.Add(action);
		}

		//----------------------------------------------------------------------------------------------------//


		#endregion Functions

	}
}
