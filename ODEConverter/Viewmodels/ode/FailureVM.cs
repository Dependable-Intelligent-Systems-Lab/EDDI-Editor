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
	public class FailureVM
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

		public FailureVM(Failure failure)
		{
			OdeFailure = failure;

			if (failure.FailureProbDistribution != null) 
			{
				FailureProbDistribution = new ProbDistVM(failure.FailureProbDistribution);
				//Items.Add(new ExpandableList("Probability Distributions") { FailureProbDistribution });
				Items.Add(FailureProbDistribution);
			}

			foreach (var @event in failure.Events)
			{
				this.Events.Add(EventVM.CreateAppropriateEventVM(@event));
			}

			Items.Add(Events);
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		private Failure OdeFailure { get; set; }

		//----------------------------------------------------------------------------------------------------//
		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Name")]
		[Description("Name")]
		public string Name { get => OdeFailure.Name; set => OdeFailure.Name = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Origin Type")]
		[Description("Origin Type")]
		public FailureOriginType OriginType { get => OdeFailure.OriginType; set => OdeFailure.OriginType = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Failure Class")]
		[Description("Failure Class")]
		public string FailureClass { get => OdeFailure.FailureClass; set => OdeFailure.FailureClass = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("FailureLogic")]
		[Description("Causal logic for the failure")]
		public string FailureLogic
		{
			get
			{
				if (OdeFailure.KeyValueMap.ContainsKey("FailureLogic"))
				{
					return OdeFailure.KeyValueMap["FailureLogic"]["value"];
				}
				return "";
			}
			set
			{
				if (OdeFailure.KeyValueMap.ContainsKey("FailureLogic"))
				{
					OdeFailure.KeyValueMap["FailureLogic"]["value"] = value;
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Unavailability")]
		[Description("Unavailability")]
		public double Unavailability { get => OdeFailure.Unavailability; set => OdeFailure.Unavailability = value; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Failure Probability Distribution")]
		[Description("Failure Probability Distribution")]
		[ExpandableObject] 
		public ProbDistVM FailureProbDistribution { get; set; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Events")]
		[Description("Events")]
		[ExpandableObject] 
		public ExpandableList Events { get; private set; } = new ExpandableList("Events");

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Caused by CCF")]
		[Description("Caused by CCF")]
		[ExpandableObject] 
		public CommonCauseFailure CausedBy { get; set; }

		//----------------------------------------------------------------------------------------------------//

		[DisplayName("Items")]
		[Description("Items")]
		[ExpandableObject]
		public ExpandableList Items { get; private set; } = new ExpandableList("Items");

		//----------------------------------------------------------------------------------------------------//

		public bool IsExpanded { get; set; }
		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		public void AddEvent(ODELib.ode.Event @event)
		{
			this.Events.Add(EventVM.CreateAppropriateEventVM(@event));

			OdeFailure.Events.Add(@event);
		}

		#endregion Functions

	}
}
