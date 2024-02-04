using ODELib.hip;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ODELib.ode
{
	public class DDI_Importer
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

		/// <summary>
		/// When we look up or load an element, cache it so we don't load it twice
		/// </summary>
		protected Dictionary<XElement, object> _elements = new Dictionary<XElement, object>();

		protected XDocument _xDoc;

		#endregion Data

		/*****************************************************************************************************/
		/* Constructors
		/*****************************************************************************************************/
		#region Constructors

		public DDI_Importer()
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
		/// Imports an ODE model from a safeTbox DDI XML file.
		/// </summary>
		/// <param name="fileName">Name of the file.</param>
		/// <returns></returns>
		public ODELib.ode.Model ImportFromDDI(string fileName)
		{
			XDocument xDocument = XDocument.Load(fileName);
			return ImportFromDDI(xDocument);
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Imports an ODE model from a safeTbox XML document.
		/// </summary>
		/// <param name="xDocument">The x document.</param>
		/// <returns></returns>
		public ODELib.ode.Model ImportFromDDI(XDocument xDocument)
		{
			_xDoc = xDocument;
			return ParseModel(xDocument);
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Registers an XML element against the object it represents for caching purposes.
		/// </summary>
		/// <param name="xe">The element.</param>
		/// <param name="obj">The object.</param>
		protected void RegisterElement(XElement xe, object obj)
		{
			_elements[xe] = obj;
		}

		//----------------------------------------------------------------------------------------------------//

		#endregion Functions

		/*****************************************************************************************************/
		/* Parse Functions
		/*****************************************************************************************************/
		#region Parse Functions

		/// <summary>
		/// Parses the model from the XML document.
		/// Note that the DDI uses a sort of reference-based system where it references using qualified names 
		/// mapped to the document structure, not simple ID numbers (though they're present), so we have to
		/// build/decode these hierarchical references to locate the right entities. 
		/// We could do a post-process phase to resolve all these references, but in this case we do a forward
		/// reference resolution process instead, caching them once we're done.
		/// </summary>
		/// <param name="ddi">The DDI XML document.</param>
		/// <returns>The ODE model.</returns>
		private Model ParseModel(XDocument ddi)
		{
			Model model = new Model();

			model.Name = ddi.Root.Attribute("name")?.Value ?? "";

			// Can have failure, system/design, hara packages
			var designPackage = from child in ddi.Root.Elements("odeProductPackages")
								where child.Attribute("name").Value == "DesignPackage"
								select child;
			var failurePackage = from child in ddi.Root.Elements("odeProductPackages")
								 where child.Attribute("name").Value == "FailureLogicPackage"
								 select child;
			var hazardPackage = from child in ddi.Root.Elements("odeProductPackages")
								where child.Attribute("name").Value == "HARAPackage"
								select child;

			var test = GetElementFromReference("//@odeProductPackages.1/@systems.1");

			// It won't let us easily look up type attributes because of the colons, so we do it the hard way
			// we need to count the packages anyway
			var systemsList = new List<System>();
			var failureModelsList = new List<FailureModel>();
			foreach (var child in ddi.Root.Elements())
			{
				switch (GetType(child))
				{
					// One of these will be the parent system; we only know because the others will be subsystems of it
					case "architecture:DesignPackage":
						// Get the systems out of it
						foreach (var systemElement in child.Elements())
						{
							var system = ParseSystem(systemElement);
							systemsList.Add(system);
						}
						break;
					
					// Failure models
					case "failureLogic_:FailureLogicPackage":
						foreach (var failureModelElement in child.Elements())
						{
							var fmodel = ParseFailureModel(failureModelElement);
							failureModelsList.Add(fmodel);
						}
						break;
				}

			}

			// Add all top-level (parentless) systems
			foreach (var system in systemsList)
			{
				if (system.Parent == null)
				{
					model.SystemElements.Add(system);
				}
			}

			// Add to the top level if they don't have a system already
			foreach (var failureModel in failureModelsList.Where(x => x.ParentSystems.Count == 0))
			{
				// Add to the top level
				model.FailureModels.Add(failureModel);
			}

			return model;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a system from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the system.</param>
		/// <returns></returns>
		private System ParseSystem(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as System;

			// This is a system
			var system = new System(xe.Attribute("name")?.Value ?? "Default");
			system.ID = GetID(xe);
			RegisterElement(xe, system);

			// These contain ports & system boundaries, signals, and optionally more subsystems
			foreach (var portElement in xe.Elements("ports"))
			{
				var port = ParsePort(portElement);
				system.Ports.Add(port);
			}
			foreach (var signalElement in xe.Elements("signals"))
			{
				var signal = ParseSignal(signalElement);
				system.Signals.Add(signal);
			}
			string subsystemsList = xe.Attribute("subSystems")?.Value ?? "";
			if (!string.IsNullOrEmpty(subsystemsList))
			{
				foreach (var subsystemElementRef in subsystemsList.Split(new char[] { ' ' }))
				{
					var subsystemElement = GetElementFromReference(subsystemElementRef);
					var subsystem = ParseSystem(subsystemElement);
					system.Subsystems.Add(subsystem);
					subsystem.Parent = system;
				}
			}
			string failureModelsList = xe.Attribute("failureModels")?.Value ?? "";
			if (!string.IsNullOrEmpty(failureModelsList))
			{
				foreach (var failureModelElementRef in failureModelsList.Split(new char[] { ' ' }))
				{
					var failureModelElement = GetElementFromReference(failureModelElementRef);
					var failureModel = ParseFailureModel(failureModelElement);
					system.FailureModels.Add(failureModel);
					failureModel.ParentSystems.Add(system);
				}
			}

			return system;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a port from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the port.</param>
		/// <returns></returns>
		private Port ParsePort(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as Port;

			var port = new Port(xe.Attribute("name")?.Value ?? $"port");
			port.ID = GetID(xe);
			RegisterElement(xe, port);

			port.FlowType = xe.Attribute("flowType")?.Value ?? "";
			port.Direction = Enum.TryParse(xe.Attribute("direction")?.Value ?? "", out PortDirection dir) ? port.Direction = dir : port.Direction = PortDirection.IN; // Seems to use IN as default

			// For interface failures we need to resolve the reference
			var failures = xe.Attribute("interfaceFailures")?.Value ?? "";
			foreach (var failureRef in failures.Split(new char[] { ' ' }))
			{
				var failureElement = GetElementFromReference(failureRef);
				var failure = ParseFailure(failureElement);
				port.InterfaceFailures.Add(failure);
			}

			return port;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a port from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the port.</param>
		/// <returns></returns>
		private Signal ParseSignal(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as Signal;

			var signal = new Signal();
			signal.ID = GetID(xe);
			RegisterElement(xe, signal);

			signal.FromPort = ParsePort(GetElementFromAttribute(xe, "fromPort"));
			signal.ToPort = ParsePort(GetElementFromAttribute(xe, "toPort"));

			return signal;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a failure model from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the failure model.</param>
		/// <returns></returns>
		private FailureModel ParseFailureModel(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as FailureModel;

			// Depends on the type...
			switch (GetType(xe))
			{
				case "failureLogic_:FaultTree":
					return ParseFaultTree(xe);
			}

			var failureModel = new FailureModel(xe.Attribute("name")?.Value ?? $"default");
			failureModel.ID = GetID(xe);
			RegisterElement(xe, failureModel);

			return failureModel;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a fault tree from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the fault tree.</param>
		/// <returns></returns>
		private FaultTree ParseFaultTree(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as FaultTree;

			var faultTree = new FaultTree(GetName(xe));
			faultTree.ID = GetID(xe);
			RegisterElement(xe, faultTree);

			// A fault tree has a bunch of failures and causes in it
			foreach (var xfailure in xe.Elements("failures"))
			{
				var failure = ParseFailure(xfailure);
				faultTree.Failures.Add(failure);
			}
			foreach (var xcause in xe.Elements("causes"))
			{
				var cause = ParseCause(xcause);
				faultTree.Causes.Add(cause);
			}
			if (faultTree.Causes.Count == 1)
			{
				faultTree.TopEvent = faultTree.Causes[0];
			}
			else
			{
				// How do we decide the top event if there's multiple causes???
				// For now, we grab the first one without a parent
				foreach (var cause in faultTree.Causes)
				{
					if (cause.Parent == null)
					{
						faultTree.TopEvent = cause;
						break;
					}
				}
			}

			return faultTree;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a failure from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the failure.</param>
		/// <returns></returns>
		private Failure ParseFailure(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as Failure;

			var failure = new Failure(GetName(xe));
			failure.ID = GetID(xe);
			RegisterElement(xe, failure);

			if (xe.Attribute("originType") != null)
			{
				failure.OriginType = (FailureOriginType)Enum.Parse(typeof(FailureOriginType), xe.Attribute("originType")?.Value ?? "", true);
			}
			failure.FailureClass = xe.Attribute("failureClass")?.Value ?? "NONE";

			// May also be a failure distribution
			if (xe.Elements("failureProbDistribution").Count() > 0)
			{
				var xprobdist = xe.Elements("failureProbDistribution").First();
				var probdist = new ProbDist(string.IsNullOrWhiteSpace(GetName(xprobdist)) ? "Probability Distribution" : GetName(xprobdist));
				failure.FailureProbDistribution = probdist;
			}

			return failure;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Parses a cause (fault tree node) from an XML element.
		/// </summary>
		/// <param name="xe">The element containing the cause.</param>
		/// <returns></returns>
		private Cause ParseCause(XElement xe)
		{
			if (_elements.ContainsKey(xe)) return _elements[xe] as Cause;

			// Is the cause a gate or a normal cause?
			Cause cause = null;
			if (GetType(xe) == "failureLogic_:Gate")
			{
				var gate = new Gate(GetName(xe));
				cause = gate;
				gate.ID = GetID(xe);
				RegisterElement(xe, gate);

				if (xe.Attribute("gateType") != null)
				{
					gate.GateType = (GateType)Enum.Parse(typeof(GateType), xe.Attribute("gateType")?.Value ?? "", true);
				}

				// May be other causes (i.e., child nodes)
				if (xe.Attribute("causes") != null)
				{
					var causes = xe.Attribute("causes").Value;
					foreach (var causeRef in causes.Split(new char[] { ' ' }))
					{
						var causeElement = GetElementFromReference(causeRef);
						var subCause = ParseCause(causeElement);
						gate.Causes.Add(subCause);
						subCause.Parent = gate;
					}
				}
			}
			else
			{
				// Just a 'normal' cause (usually a basic event)
				cause = new Cause(GetName(xe));
				cause.ID = GetID(xe);
				RegisterElement(xe, cause);
			}

			if (xe.Attribute("causeType") != null)
			{
				cause.CauseType = (CauseType)Enum.Parse(typeof(CauseType), xe.Attribute("causeType")?.Value ?? "", true);
			}

			// May be failure e.g. if it's a basic event
			if (xe.Attribute("failure") != null)
			{
				string failureRef = xe.Attribute("failure").Value;
				var xfailure = GetElementFromReference(failureRef);
				var failure = ParseFailure(xfailure);
				cause.Failure = failure;
			}

			return cause;
		}

		//----------------------------------------------------------------------------------------------------//


		#endregion Parse Functions


		/*****************************************************************************************************/
		/* Lookup Functions
		/*****************************************************************************************************/
		#region Lookup Functions

		/// <summary>
		/// Cross-references are usually stored in attributes; this extracts the reference from the attribute, 
		/// then looks up the element it represents.
		/// </summary>
		/// <param name="xe">The XML element containing the attribute.</param>
		/// <param name="attribName">Name of the attribute.</param>
		/// <returns></returns>
		public XElement GetElementFromAttribute(XElement xe, string attribName)
		{
			return GetElementFromReference(xe.Attribute(attribName).Value);
		}

		//----------------------------------------------------------------------------------------------------//


		/// <summary>
		/// This convoluted mess looks up a given XElement from the weird path reference style safeTbox/EA uses.
		/// Essentially, a cross-reference is of the format '//@element.index/@subelement.index', so this
		/// breaks it down hierarchically and looks up each element until it reaches the right one.
		/// </summary>
		/// <param name="reference">The reference to decode.</param>
		/// <returns></returns>
		public XElement GetElementFromReference(string reference)
		{
			// A reference is of the format "//@element.index/@subelement.index"
			// The root is an integration package, under which are odeProductPackages (the top-level elements)

			// Internal function to allow recursion
			XElement GetElementFromElement(XElement xe, string[] remainingReference, int depth=1)
			{
				if (depth >= remainingReference.Length)
				{
					return xe;
				}

				var children = xe.Elements(GetReferenceName(remainingReference[depth]));
				int subindex = GetReferenceIndex(remainingReference[depth]);
				int j = 0;
				foreach (var child in children)
				{
					if (j == subindex)
					{
						return GetElementFromElement(child, remainingReference, depth + 1);
					}
					j++;
				}
				return xe;
			}

			// Extract the name from the reference
			string GetReferenceName(string refPart)
			{
				int start = refPart.IndexOf("@") + 1;
				int end = refPart.IndexOf(".");
				return refPart.Substring(start, end - start);
			}

			// Extract the index from the reference
			int GetReferenceIndex(string refPart)
			{
				int start = refPart.IndexOf(".");
				return int.Parse(refPart.Substring(start + 1));
			}

			// Break up the reference and look up the top-level element
			string[] parts = reference.Substring(2).Split(new char[] { '/' }); // Skip the // and split on /
			var packages = _xDoc.Root.Elements(GetReferenceName(parts[0]));
			int index = GetReferenceIndex(parts[0]);
			int i = 0;
			foreach (var child in packages)
			{
				if (i == index)
				{
					return GetElementFromElement(child, parts, 1);
				}
				i++;
			}

			return null;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Gets the type of an ODE element.
		/// Since these are typed as 'xsi:type' they can't be accessed directly via string, so we need to build
		/// the xname with the namespace first.
		/// </summary>
		/// <param name="xe">The xe.</param>
		/// <returns></returns>
		public static string GetType(XElement xe)
		{
			XName name = XName.Get("type", "http://www.w3.org/2001/XMLSchema-instance");
			return xe.Attributes(name).FirstOrDefault()?.Value ?? "";
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Gets the ID number as a long.
		/// </summary>
		/// <param name="xe">The xe.</param>
		/// <returns></returns>
		public static long GetID(XElement xe)
		{
			if (long.TryParse(xe.Attribute("Id")?.Value ?? "-1", out long result))
			{
				return result;
			}
			return -1;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Gets the name of an element.
		/// </summary>
		/// <param name="xe">The xe.</param>
		/// <param name="default">The default.</param>
		/// <returns></returns>
		public static string GetName(XElement xe, string @default="Default")
		{
			return xe.Attribute("name")?.Value ?? @default;
		}

		//----------------------------------------------------------------------------------------------------//

		/// <summary>
		/// Generates a hierarchical reference ID for a given element.
		/// </summary>
		/// <param name="xe">The element.</param>
		/// <returns>A string of the form //@element.index/subelement.index etc</returns>
		public static string GenerateHierarchicalID(XElement xe)
		{
			var parent = xe.Parent;
			if (parent == null)
			{
				// This is root
				return "/";
			}

			string hid = $"@{xe.Name.LocalName}";
			int index = 0;
			foreach (var sibling in parent.Elements(xe.Name))
			{
				if (sibling == xe)
				{
					break;
				}
				index++;
			}
			hid += $".{index}";

			// Include parent
			hid = GenerateHierarchicalID(parent) + "/" + hid;

			return hid;
		}

		#endregion Lookup Functions
	}
}
