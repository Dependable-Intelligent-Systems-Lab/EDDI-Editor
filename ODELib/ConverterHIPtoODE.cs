using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib
{
	/// <summary>
	/// Converter for transforming HiP-HOPS models (architecture, fault trees, FMEAs) into ODE models.
	/// </summary>
	/// <seealso cref="ODELib.Converter" />
	public class ConverterHIPtoODE : Converter
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

		public ConverterHIPtoODE()
			: base(ConverterType.HIPHOPS_TO_ODE)
		{
		}

		#endregion Constructors

		/*****************************************************************************************************/
		/* Properties
		/*****************************************************************************************************/
		#region Properties

		#endregion Properties

		/*****************************************************************************************************/
		/* Functions
		/*****************************************************************************************************/
		#region Functions

		/// <summary>
		/// Converts a HiP-HOPS model (the top-level container) into an ODE model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public ode.Model ConvertHIPtoODE(hip.Model model)
		{
			if (model == null)
			{
				return null;
			}

			var outputModel = new ode.Model();

			// Hazards first, because they may be back-referenced from perspectives
			foreach (var hazard in model.Hazards)
			{
				var ode_hazard = new ode.Hazard(hazard.Name);
				ode_hazard.Condition = hazard.Description;
				foreach (var cause in hazard.Causes)
				{
					var ode_failure = new ode.Failure(cause.FailureLogic);
					ode_hazard.Failures.Add(ode_failure);
				}
				outputModel.Hazards.Add(ode_hazard);
			}

			// Then perspectives (which get converted to Systems)
			var systemDict = new Dictionary<hip.Perspective, ode.System>();
			foreach (var perspective in model.Perspectives)
			{
				// Each of these becomes a top-level 'system' 
				var ode_system = ConvertSystem(perspective.System);
				ode_system.Name = perspective.Name;
				outputModel.SystemElements.Add(ode_system);
				systemDict[perspective] = ode_system;
			}

			foreach (var perspective in model.Perspectives)
			{
				// Deal with ports and lines (they have to be done last, because we need all ports to exist,
				// hence the repeated loop)
				ConvertSystemLines(perspective.System, systemDict[perspective]);
			}

			// If results are also present, convert those too
			if (model.Results != null)
			{
				ConvertAnalysisResults(model.Results, outputModel);
			}

			return outputModel;
		}

		//----------------------------------------------------------------------------------------------------//

		public override ConverterModel Convert(ConverterModel inputModel)
		{
			return ConvertHIPtoODE(inputModel as hip.Model) as ode.Model;
		}

		#endregion Functions

		/*****************************************************************************************************/
		/* Conversion Functions
		/*****************************************************************************************************/
		#region Conversion Functions

		/// <summary>
		/// Converts the HH system into an ODE one.
		/// </summary>
		/// <param name="hip_system">The hip system.</param>
		/// <returns></returns>
		private ode.System ConvertSystem(hip.System hip_system)
		{
			if (hip_system == null)
			{
				return null;
			}

			// We actually skip the perspective here and go straight for system
			var ode_system = new ode.System(hip_system.Name);
		
			foreach (var hip_component in hip_system.Components)
			{
				var ode_component = ConvertComponent(hip_component);
				ode_system.Subsystems.Add(ode_component);
			}

			return ode_system;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts the HH component into an ODE one.
		/// </summary>
		/// <param name="hip_component">The hip component.</param>
		/// <returns></returns>
		private ode.System ConvertComponent(hip.Component hip_component)
		{
			if (hip_component == null)
			{
				return null;
			}

			var ode_system = new ode.System(hip_component.Name);

			// Convert ports
			foreach (var hip_port in hip_component.Ports)
			{
				var ode_port = new ode.Port(hip_port.Name);
				switch (hip_port.Type)
				{
					case "Inport":
						ode_port.Direction = ode.PortDirection.IN;
						break;
					case "Outport":
						ode_port.Direction = ode.PortDirection.OUT;
						break;
					case "Both":
						ode_port.Direction = ode.PortDirection.BOTH;
						break;
				}
				ode_system.Ports.Add(ode_port);
			}

			// Convert implementation
			if (hip_component.Implementations.Count > 0)
			{
				var hip_implementation = hip_component.Implementations[0];

				// Deal with HBlockType by putting it in the KVM
				ode_system.KeyValueMap["HBlockType"] = new SerialisableDictionary<string, string>();
				ode_system.KeyValueMap["HBlockType"]["value"] = hip_implementation.HBlockType.ToString();

				// Failure data is the next big one
				ConvertFailureData(hip_component, hip_implementation, ode_system);

				// Deal with any subsystem
				if (hip_implementation.System != null &&
					hip_implementation.System.Components.Count > 0) // Only if there are components (since Matlab likes making empty subsystems)
				{
					var subsystem = ConvertSystem(hip_implementation.System);
					ode_system.Subsystems.Add(subsystem);
				}
			}

			return ode_system;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts the component failure data (basic events, output deviations) into an ODE Failure Model.
		/// </summary>
		/// <param name="hip_component">The hip component.</param>
		/// <param name="hip_implementation">The hip implementation.</param>
		/// <param name="ode_system">The ode system.</param>
		private void ConvertFailureData(hip.Component hip_component, hip.Implementation hip_implementation, ode.System ode_system)
		{
			if (hip_implementation == null || ode_system == null || hip_implementation.FailureData == null)
			{
				return;
			}

			var hip_failureData = hip_implementation.FailureData;
			var failureModel = new ode.FailureModel("Component Failure Data"); // Make a generic top-level failure model for this component (acting as component failure data)
			ode_system.FailureModels.Add(failureModel);


			// The ODE stores its ODEVNs in its Ports
			foreach (var odevn in hip_failureData.OutputDeviations)
			{
				var ode_failure = new ode.Failure(odevn.FullName);
				var odevnParts = odevn.Name.Split(new char[] { '-' });

				ode_failure.FailureClass = odevnParts[0];
				ode_failure.KeyValueMap["FailureLogic"] = new SerialisableDictionary<string, string>();
				ode_failure.KeyValueMap["FailureLogic"]["value"] = odevn.Causes[0].FailureLogic; 
				ode_failure.OriginType = ode.FailureOriginType.Output;

				// Add to port
				var portName = odevnParts[1];
				var ode_port = ode_system.GetPort(portName);
				ode_port.InterfaceFailures.Add(ode_failure);

				// And to failure model
				failureModel.Failures.Add(ode_failure);
			}

			// Now for basic events
			foreach (var basicEvent in hip_failureData.BasicEvents)
			{
				var ode_failure = ConvertBasicEvent(basicEvent);

				failureModel.Failures.Add(ode_failure);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a HH basic event into an ODE Failure.
		/// </summary>
		/// <param name="basicEvent">The basic event.</param>
		/// <returns></returns>
		private ode.Failure ConvertBasicEvent(hip.BasicEvent basicEvent)
		{
			var ode_failure = new ode.Failure(basicEvent.FullName);
			ode_failure.OriginType = ode.FailureOriginType.Internal;

			if (basicEvent.UnavailabilityFormula != null)
			{
				var formula = basicEvent.UnavailabilityFormula;
				if (formula.UnavailabilityFormula_Constant != null)
				{
					var ufc = formula.UnavailabilityFormula_Constant;
					ode_failure.FailureProbDistribution = new ode.ProbDist("Constant");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Failure Rate", ufc.FailureRate.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Repair Rate", ufc.RepairRate.ToString()));
				}
				else if (formula.UnavailabilityFormula_MTTF != null)
				{
					var ufc = formula.UnavailabilityFormula_MTTF;
					ode_failure.FailureProbDistribution = new ode.ProbDist("MTTF");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("MTTF", ufc.MTTF.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Repair Rate", ufc.RepairRate.ToString()));
				}
				else if (formula.UnavailabilityFormula_MTTR != null)
				{
					var ufc = formula.UnavailabilityFormula_MTTR;
					ode_failure.FailureProbDistribution = new ode.ProbDist("MTTR");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Failure Rate", ufc.FailureRate.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("MTTR", ufc.MTTR.ToString()));
				}
				else if (formula.UnavailabilityFormula_MTTF_MTTR != null)
				{
					var ufc = formula.UnavailabilityFormula_MTTF_MTTR;
					ode_failure.FailureProbDistribution = new ode.ProbDist("MTTFMTTR");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("MTTF", ufc.MTTF.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("MTTR", ufc.MTTR.ToString()));
				}
				else if (formula.UnavailabilityFormula_Fixed != null)
				{
					var ufc = formula.UnavailabilityFormula_Fixed;
					ode_failure.FailureProbDistribution = new ode.ProbDist("Fixed");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Unavailability", ufc.Unavailability.ToString()));
				}
				else if (formula.UnavailabilityFormula_Dormant != null)
				{
					var ufc = formula.UnavailabilityFormula_Dormant;
					ode_failure.FailureProbDistribution = new ode.ProbDist("Dormant");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Failure Rate", ufc.FailureRate.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("MTTR", ufc.MTTR.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("T", ufc.T.ToString()));
				}
				else if (formula.UnavailabilityFormula_Poisson != null)
				{
					var ufc = formula.UnavailabilityFormula_Poisson;
					ode_failure.FailureProbDistribution = new ode.ProbDist("Poisson");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("FailureRate", ufc.FailureRate.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("N", ufc.N.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("S", ufc.S.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("T", ufc.T.ToString()));
				}
				else if (formula.UnavailabilityFormula_Binomial != null)
				{
					var ufc = formula.UnavailabilityFormula_Binomial;
					ode_failure.FailureProbDistribution = new ode.ProbDist("Binomial");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("FailureRate", ufc.FailureRate.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("RepairRate", ufc.RepairRate.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("N", ufc.N.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("M", ufc.M.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("T", ufc.T.ToString()));
				}
				else if (formula.UnavailabilityFormula_Variable != null)
				{
					var ufc = formula.UnavailabilityFormula_Variable;
					ode_failure.FailureProbDistribution = new ode.ProbDist("Variable");
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Slope1", ufc.Slope1.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Scale1", ufc.Scale1.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Interval1", ufc.Interval1.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Scale2", ufc.Scale2.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Interval2", ufc.Interval2.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Slope3", ufc.Slope3.ToString()));
					ode_failure.FailureProbDistribution.Parameters.Add(new ode.ProbDistParam("Scale3", ufc.Scale3.ToString()));
				}
			}

			return ode_failure;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts the HH lines into ODE signals.
		/// </summary>
		/// <param name="hip_system">The hip system.</param>
		/// <param name="ode_system">The ode system.</param>
		private void ConvertSystemLines(hip.System hip_system, ode.System ode_system)
		{
			// This is a bit hacky, as HiP-HOPS lines are stupidly complex while the ODE's are simple unidirectional point-to-point signals.
			// Essentially we break down any one-to-many/many-to-one/many-to-many HH line into individual signals. However, we lose any
			// unusual line logic in the process (e.g. different AND/OR logic per failure class), since that isn't directly supported in the
			// ODE. Fortunately, it's rare that such logic exists in HiP-HOPS (and indeed cannot be created in Simulink anyway).
			foreach (var hip_line in hip_system.Lines)
			{
				var ode_signals = ConvertLine(hip_line, ode_system);
				foreach (var signal in ode_signals)
				{
					ode_system.Signals.Add(signal);
				}
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a single line into ODE signals.
		/// </summary>
		/// <param name="line">The line.</param>
		/// <param name="ode_system">The ode system.</param>
		/// <returns></returns>
		private List<ode.Signal> ConvertLine(hip.Line line, ode.System ode_system)
		{
			if (line == null || line.Connections.Count == 0)
			{
				return null;
			}

			// This is a pain
			// A HiP-HOPS line consists of a number of port expressions, one per port connected by the line.
			// Depending on the type of line, any port may be an input port, output port, or both.
			// A line may also have default connections rather than explicitly specifying them, and it may also
			// have logic specific to different failure classes.
			// As mentioned above, we simplify this here by stripping out any unusual logic to obtain one-to-one signals.
			// However, to convert to ODE, we therefore need to build the full line port expression array ourselves so we can
			// convert to single Signals.

			// Worse, we can only do this once all the ports exist...

			var signals = new List<ode.Signal> ();

			foreach (var connection in line.Connections)
			{
				var destinationPortName = connection.Port;
				var portExp = connection.PortExpression;
				
				// TODO: We should really parse the port expression, but that means implementing a full expression parser.
				// Since we can't actually handle the logic anyway, for now we just interpret it as a single port.
				// This should work for any Matlab Simulink-created model (since only 1:1 or 1:N lines are possible) but
				// will probably fail for many SimulationX models with undirected lines.
				var fromPort = ode_system.GetPort(portExp);
				var toPort = ode_system.GetPort(destinationPortName);

				var signal = new ode.Signal()
				{
					FromPort = fromPort,
					ToPort = toPort
				};

				signals.Add(signal);
			}

			return signals;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts the analysis results (FTA, FMEA) into ODE failure models.
		/// </summary>
		/// <param name="results">The results.</param>
		/// <param name="ode_model">The ode model.</param>
		private void ConvertAnalysisResults(hip.HipResults results, ode.Model ode_model)
		{
			// We assume there is only one perspective here (and thus one top-level system in the model)
			var ode_system = ode_model.SystemElements.First();
			
			foreach (var hip_ft in results.FaultTrees)
			{
				var ode_ft = ConvertFaultTree(hip_ft, ode_model);
				ode_system.FailureModels.Add(ode_ft);
			}

			// We do the FMEA second so it can find the top failures in the FTs
			var fmea = ConvertFMEA(results.FMEA, ode_model);
			ode_system.FailureModels.Add(fmea);
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a HH FMEA into an ODE FMEA.
		/// </summary>
		/// <param name="hip_fmea">The hip fmea.</param>
		/// <param name="ode_model">The ode model.</param>
		/// <returns></returns>
		private ode.FMEA ConvertFMEA(hip.FMEA hip_fmea, ode.Model ode_model)
		{
			var ode_fmea = new ode.FMEA("System FMEA");
			
			foreach (var component in hip_fmea.Components)
			{
				foreach (var e in component.Events)
				{
					foreach (var effect in e.Effects)
					{
						var fmeaEntry = new ode.FMEAEntry();
						fmeaEntry.Name = component.Name;
						fmeaEntry.Mode = ode_model.LookupFailure(e.Name);

						// Effects are hazards and/or fault trees in HH, but here they are just failures.
						// So we lookup the existing fault tree top 'failure' with this name.
						foreach (var fm in ode_model.SystemElements.First().FailureModels)
						{
							if (fm is ode.FaultTree ft)
							{
								fmeaEntry.Effect = ft.TopEvent.Failure;
							}
						}

						ode_fmea.Entries.Add(fmeaEntry);
					}
				}
			}

			return ode_fmea;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a HH Fault Tree into an ODE Fault Tree.
		/// </summary>
		/// <param name="hip_faultTree">The hip fault tree.</param>
		/// <param name="ode_model">The ode model.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">Could not look up failure: " + be.BasicEvent.OriginalBasicEvent.FullName</exception>
		private ode.FaultTree ConvertFaultTree(hip.FaultTree hip_faultTree, ode.Model ode_model)
		{
			var ode_faultTree = new ode.FaultTree(hip_faultTree.Name);

			// Deal with structure
			if (hip_faultTree.TopNode.Count > 0)
			{
				var top = ConvertFaultTreeNode(hip_faultTree.TopNode.First(), ode_model);
				ode_faultTree.TopEvent = top;
			}

			// Give top node a special failure that acts as the top-level failure
			ode.Failure topFailure = null;
			if (ode_faultTree.TopEvent != null)
			{
				topFailure                     = new ode.Failure(hip_faultTree.Name);
				topFailure.Unavailability      = hip_faultTree.Unavailability;
				ode_faultTree.TopEvent.Failure = topFailure;

				// Create an action event for this
				var warningAction = new ODELib.ode.WarningAction(hip_faultTree.Name)
				{
					Warning = hip_faultTree.Name,
					WarningType = "event"
				};
				ode_faultTree.TopEvent.Actions.Add(warningAction);
			}

			// And now the MCS
			var ode_cutsetList = new ode.MinimalCutSets();
			ode_cutsetList.Failure = topFailure;
			foreach (var cutsetsList in hip_faultTree.AllCutSets)
			{
				foreach (var cutset in cutsetsList.CutSets)
				{
					var ode_cutset = new ode.MinimalCutSet();
					ode_cutsetList.CutSets.Add(ode_cutset);

					foreach (var be in cutset.Events)
					{
						var ode_cse = new ode.CutSetEvent();

						// Lookup failure from FMEA via model
						if (be.BasicEvent.OriginalBasicEvent != null)
						{
							var existingFailure = ode_model.LookupFailure(be.BasicEvent.OriginalBasicEvent.FullName);
							ode_cse.Failure = existingFailure ?? throw new Exception("Could not look up failure: " + be.BasicEvent.OriginalBasicEvent.FullName);
							ode_cutset.Causes.Add(ode_cse);
						}
					}
				}
			}
			ode_faultTree.MinimalCutSets.Add(ode_cutsetList);

			return ode_faultTree;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts a HH fault tree node into an ODE Cause.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="ode_model">The ode model.</param>
		/// <returns></returns>
		/// <exception cref="System.Exception">Cause not a recognised type</exception>
		private ode.Cause ConvertFaultTreeNode(hip.FaultTreeNode node, ode.Model ode_model)
		{
			if (node is hip.OrGate || node is hip.AndGate)
			{
				var ode_cause = new ode.Gate(node.Name);

				ode_cause.CauseType = ode.CauseType.Gate;
				
				if (node is hip.OrGate)
				{
					ode_cause.GateType = ode.GateType.OR;
				}
				else if (node is hip.AndGate)
				{
					ode_cause.GateType = ode.GateType.AND;
				}

				foreach (var child in node.Children)
				{
					var childCause = ConvertFaultTreeNode(child, ode_model);
					ode_cause.Causes.Add(childCause);
				}

				return ode_cause;
			}
			else if (node is hip.BasicEventRef be)
			{
				var ode_cause = new ode.Cause(node.Name);

				ode_cause.Failure = ode_model.LookupFailure(be.BasicEvent.OriginalBasicEvent.FullName);
				ode_cause.CauseType = ode.CauseType.BasicEvent;

				// Create a default event that triggers when the basic event does
				var warningEvent = new ode.WarningAction(be.BasicEvent.OriginalBasicEvent.FullName)
				{
					Warning = be.BasicEvent.OriginalBasicEvent.FullName,
					WarningType = "event"
				};
				ode_cause.Actions.Add(warningEvent);

				return ode_cause;
			}
			throw new Exception("Cause not a recognised type");
		}

		//----------------------------------------------------------------------------------------------------//


		#endregion Conversion Functions
	}
}
