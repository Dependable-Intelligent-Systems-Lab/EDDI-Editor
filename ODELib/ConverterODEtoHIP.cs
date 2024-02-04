using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ODELib
{
	/// <summary>
	/// Converter for transforming ODE models into HiP-HOPS models.
	/// 
	/// Conversion from ODE to HiP-HOPS is complicated by the fact that there are two types of HiP-HOPS files;
	/// one contains the model information and the other contains the analysis results (i.e., FTAs + FMEA).
	/// We can convert the model to the internal representation, but saving them becomes problematic since we
	/// can't easily just serialise it to one file.
	/// 
	/// Conversion is further complicated by the fact that HiP-HOPS is essentially a subset of the ODE, so 
	/// things in the ODE that aren't in HiP-HOPS just get ignored.
	/// 
	/// As a result, this is not fully supported.
	/// </summary>
	/// <seealso cref="ODELib.Converter" />
	public class ConverterODEtoHIP : Converter
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

		public ConverterODEtoHIP()
			: base(ConverterType.ODE_TO_HIPHOPS)
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

		public hip.Model ConvertODEtoHIP(ode.Model model)
		{
			if (model == null)
			{
				return null;
			}

			var outputModel = new hip.Model();

			// Architecture first
			foreach (var sysperspective in model.SystemElements)
			{
				var perspective = ConvertSystemToPerspective(sysperspective);
				outputModel.Perspectives.Add(perspective);

				// Also deal with lines and stuff
				ConvertSystemLines(sysperspective, perspective.System);
			}

			// Then hazards
			foreach (var hazard in model.Hazards)
			{
				var hipHazard = new hip.Hazard
				{
					Name = hazard.Name,
					Description = hazard.Condition
				};
				foreach (var failure in hazard.Failures)
				{
					var cause = new hip.Cause()
					{
						FailureLogic = failure.Name
					};
					hipHazard.Causes.Add(cause);
				}
				outputModel.Hazards.Add(hipHazard);
			}

			// And results
			if (model.SystemElements.Count > 0)
			{
				var top = model.SystemElements.First();
				if (top.FailureModels.Count > 0)
				{
					// TODO: Convert failure models to FMEA/FTA
				}
			}

			return outputModel;
		}

		//----------------------------------------------------------------------------------------------------//

		public override ConverterModel Convert(ConverterModel inputModel)
		{
			return ConvertODEtoHIP(inputModel as ode.Model) as hip.Model;
		}

		#endregion Functions

		/*****************************************************************************************************/
		/* Conversion Functions
		/*****************************************************************************************************/
		#region Conversion Functions

		/// <summary>
		/// Converts an ODE System to a HiP-HOPS perspective. Only for top-level systems, since Perspectives are
		/// also only top-level.
		/// Note that this may not result in a perfectly reversible transformation, since it assumes there are 
		/// always Perspectives at the top level and there is no direct equivalent to a HiP-HOPS Perspective in 
		/// the ODE.
		/// </summary>
		/// <param name="odePerspective">The ode perspective.</param>
		/// <returns></returns>
		private hip.Perspective ConvertSystemToPerspective(ode.System odePerspective)
		{
			var hipPerspective = new hip.Perspective()
			{
				Name = odePerspective.Name,
				System = new hip.System()
			};

			foreach (var component in odePerspective.Subsystems)
			{
				var hipComponent = ConvertSystemToComponent(component);
				hipPerspective.System.Components.Add(hipComponent);
			}

			return hipPerspective;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts the an ODE System to a HiP-HOPS component.
		/// </summary>
		/// <param name="odeComponent">The ode component.</param>
		/// <returns></returns>
		private hip.Component ConvertSystemToComponent(ode.System odeComponent)
		{
			var hipComponent = new hip.Component()
			{
				Name = odeComponent.Name
			};

			// Convert ports
			foreach (var port in odeComponent.Ports)
			{
				var hipPort = new hip.Port()
				{
					Name = port.Name
				};

				switch (port.Direction)
				{
					case ode.PortDirection.BOTH:
						hipPort.Type = "Both";
						break;
					case ode.PortDirection.IN:
						hipPort.Type = "Inport";
						break;
					case ode.PortDirection.OUT:
						hipPort.Type = "Outport";
						break;
				}
				hipComponent.Ports.Add(hipPort); // Why does this make me think of hip replacement surgery...
			}

			// Implementations
			var implementation = new hip.Implementation() { Name = "MainImplementation" };

			// Hblocktype -- decides how failures propagate from subsystems
			implementation.HBlockType = (hip.HBlockType)Enum.Parse(typeof(hip.HBlockType), odeComponent.KeyValueMap["HBlockType"]["value"]);

			// Failure data
			ConvertFailureData(odeComponent, hipComponent, implementation);

			// Subsystems?
			if (odeComponent.Subsystems.Count > 0)
			{
				implementation.System = new hip.System()
				{
					Name = odeComponent.Name
				};
			}
			foreach (var subComponent in odeComponent.Subsystems)
			{
				implementation.System.Components.Add(ConvertSystemToComponent(subComponent));

				ConvertSystemLines(subComponent, implementation.System);
			}

			hipComponent.Implementations.Add(implementation);

			return hipComponent;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts the ODE failure data to appropriate HiP-HOPS entities.
		/// </summary>
		/// <param name="odeComponent">The ode component.</param>
		/// <param name="hipComponent">The hip component.</param>
		/// <param name="impl">The implementation.</param>
		private void ConvertFailureData(ode.System odeComponent, hip.Component hipComponent, hip.Implementation impl)
		{
			var failureData = new hip.FailureData();
			impl.FailureData = failureData;

			if (odeComponent.FailureModels.Count == 0)
			{
				return;
			}

			var failureModel = odeComponent.FailureModels.First();

			// ODE stores ODEVNs in Ports, so we must fetch them from there
			foreach (var odePort in odeComponent.Ports)
			{
				// Get corresponding hipPort
				hip.Port hipPort = null;
				foreach (var port in hipComponent.Ports)
				{
					if (odePort.Name == port.Name)
					{
						hipPort = port;
					}
				}

				if (hipPort == null)
				{
					continue; // Could not find the right port, so skip
				}

				foreach (var doubleodevn in odePort.InterfaceFailures) // doubleodevn = ode.odevn = 007 -- geddit? Nah, not that funny anyway
				{
					var odevnParts = doubleodevn.Name.Split(new char[] { '-' });

					var hipODEVN = new hip.OutputDeviation()
					{
						Name = doubleodevn.Name,
						ParentPort = hipPort
					};
					var cause = new hip.Cause()
					{
						FailureLogic = doubleodevn.KeyValueMap["FailureLogic"]["value"]
					};

					hipPort.OutputDeviations.Add(hipODEVN);
					failureData.OutputDeviations.Add(hipODEVN);
				}
			}

			// Basic Events
			foreach (var failure in failureModel.Failures)
			{
				var be = ConvertBasicEvent(failure);
				impl.FailureData.BasicEvents.Add(be);
			}
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts an ODE Failure to a HiP-HOPS basic event.
		/// </summary>
		/// <param name="failure">The failure.</param>
		/// <returns></returns>
		private hip.BasicEvent ConvertBasicEvent(ode.Failure failure)
		{
			var be = new hip.BasicEvent()
			{
				FullName = failure.Name
			};

			if (failure.FailureProbDistribution != null)
			{
				if (failure.FailureProbDistribution.Name == "Constant")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_Constant = new hip.UnavailabilityFormula_Constant();
					uf.UnavailabilityFormula_Constant.FailureRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Failure Rate").First().Value);
					uf.UnavailabilityFormula_Constant.RepairRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Repair Rate").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "MTTF")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_MTTF = new hip.UnavailabilityFormula_MTTF();
					uf.UnavailabilityFormula_MTTF.MTTF = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "MTTF").First().Value);
					uf.UnavailabilityFormula_MTTF.RepairRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Repair Rate").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "MTTR")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_MTTR = new hip.UnavailabilityFormula_MTTR();
					uf.UnavailabilityFormula_MTTR.MTTR = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "MTTR").First().Value);
					uf.UnavailabilityFormula_MTTR.FailureRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Failure Rate").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "MTTFMTTR")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_MTTF_MTTR = new hip.UnavailabilityFormula_MTTF_MTTR();
					uf.UnavailabilityFormula_MTTF_MTTR.MTTR = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "MTTR").First().Value);
					uf.UnavailabilityFormula_MTTF_MTTR.MTTF = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "MTTF").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "Fixed")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_Fixed = new hip.UnavailabilityFormula_Fixed();
					uf.UnavailabilityFormula_Fixed.Unavailability = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Unavailability").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "Dormant")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_Dormant = new hip.UnavailabilityFormula_Dormant();
					uf.UnavailabilityFormula_Dormant.FailureRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Failure Rate").First().Value);
					uf.UnavailabilityFormula_Dormant.MTTR = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "MTTR").First().Value);
					uf.UnavailabilityFormula_Dormant.T = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "T").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "Poisson")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_Poisson = new hip.UnavailabilityFormula_Poisson();
					uf.UnavailabilityFormula_Poisson.FailureRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Failure Rate").First().Value);
					uf.UnavailabilityFormula_Poisson.N = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "N").First().Value);
					uf.UnavailabilityFormula_Poisson.S = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "S").First().Value);
					uf.UnavailabilityFormula_Poisson.T = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "T").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "Binomial")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_Binomial = new hip.UnavailabilityFormula_Binomial();
					uf.UnavailabilityFormula_Binomial.FailureRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Failure Rate").First().Value);
					uf.UnavailabilityFormula_Binomial.RepairRate = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Repair Rate").First().Value);
					uf.UnavailabilityFormula_Binomial.N = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "N").First().Value);
					uf.UnavailabilityFormula_Binomial.M = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "M").First().Value);
					uf.UnavailabilityFormula_Binomial.T = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "T").First().Value);
					be.UnavailabilityFormula = uf;
				}
				else if (failure.FailureProbDistribution.Name == "Variable")
				{
					var uf = new hip.UnavailabilityFormula();
					uf.UnavailabilityFormula_Variable = new hip.UnavailabilityFormula_Variable();
					uf.UnavailabilityFormula_Variable.Scale1 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Slope1").First().Value);
					uf.UnavailabilityFormula_Variable.Slope1 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Scale1").First().Value);
					uf.UnavailabilityFormula_Variable.Interval1 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Interval1").First().Value);
					uf.UnavailabilityFormula_Variable.Scale2 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Scale2").First().Value);
					uf.UnavailabilityFormula_Variable.Interval2 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Interval2").First().Value);
					uf.UnavailabilityFormula_Variable.Slope3 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Slope3").First().Value);
					uf.UnavailabilityFormula_Variable.Scale3 = double.Parse(failure.FailureProbDistribution.Parameters.Where(x => x.Name == "Scale3").First().Value);
					be.UnavailabilityFormula = uf;
				}
			}

			return be;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Converts ODE signals to HiP-HOPS lines.
		/// </summary>
		/// <param name="odeSystem">The ode system.</param>
		/// <param name="hipSystem">The hip system.</param>
		private void ConvertSystemLines(ode.System odeSystem, hip.System hipSystem)
		{
			// HiP-HOPS lines are complex but since we're just converting from the ODE, we have an easier start
			// as we only ever use 1:1 signals as input. Thus each signal is basically a single connection in a
			// single line. However, HiP-HOPS only allows one line per port (technically more if subsystems
			// are involved) so we package these as multiple connections per line.

			// Since we're just using string names everywhere, we do virtually no checking here. Validation
			// should ensure the ports referred to (in both models) actually exist.

			// We need to know all the destination ports first
			var destinationPorts = new Dictionary<string, List<string>>(); // Destination, Sources
			foreach (var signal in odeSystem.Signals)
			{
				if (!destinationPorts.ContainsKey(signal.ToPort.Name))
				{
					destinationPorts[signal.ToPort.Name] = new List<string>();
				}
				destinationPorts[signal.ToPort.Name].Add(signal.FromPort.Name);
			}

			// Now convert signals to lines 
			for (int i = 0; i < destinationPorts.Keys.Count; i++) 
			{
				var dstPort = destinationPorts.Keys.First();
				var dstPortSources = destinationPorts[dstPort];
				destinationPorts.Remove(dstPort);

				// Create the connection.
				var line = new hip.Line();
				var connection = new hip.Connection();
				connection.Port = dstPort;
				line.Connections.Add(connection);

				// We cheat a bit here. Technically HiP-HOPS can use more complex logic but the ODE does not, so we assume that 
				// multiple source ports connected to the same destination are part of a disjunction (OR) relationship.
				foreach (var srcPort in dstPortSources)
				{
					connection.PortExpression += srcPort;
					if (srcPort != dstPortSources.Last())
					{
						connection.PortExpression += " OR ";
					}
				}

				// Now for the complex bit. Each destination port starts its own Connection (i.e. Port/PortExpression combo)
				// but if one of the sources is also a destination, e.g. as in a bi-directional or multidirectional line, 
				// then we have to package all of them into the same line. (The ODE does this much more simply, I admit.)
				foreach (var srcPort in dstPortSources)
				{
					if (destinationPorts.Keys.Contains(srcPort))
					{
						// Okay, the source is also a destination. That means it needs to be part of the same line, as a new connection.
						// First, remove it from the list so it doesn't become a new line later.
						var srcPortSources = destinationPorts[srcPort];
						destinationPorts.Remove(srcPort);

						// Create a new connection in the line
						var srcConnection = new hip.Connection();
						srcConnection.Port = srcPort;

						foreach (var srcPortSrc in srcPortSources)
						{
							srcConnection.PortExpression += srcPortSrc;
							if (srcPort != srcPortSources.Last())
							{
								srcConnection.PortExpression += " OR ";
							}
						}

						line.Connections.Add(srcConnection);
					}
				}

				hipSystem.Lines.Add(line);
			}
		}

		//----------------------------------------------------------------------------------------------------//



		#endregion Conversion Functions
	}
}
