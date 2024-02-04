# EDDI EDITOR
The EDDI Editor is a small tool intended to act as glue between different safety analysis tools and runtime EDDI generators. It can:
* Import HiP-HOPS models (both system architectures and analysis results, i.e., FMEA/FTA) and convert them to ODE models
* Import Dymodia state machines and convert them to ODE models
* Load ODE models directly (currently, system architectures, fault trees, FMEAs, and state machines), from XML and DDI (safeTbox) files
* Allow rudimentary editing of most properties of the entities in each model (both before and after conversion)
* Merge ODE models, e.g. by importing a model/subsystem hierarchy to replace an existing (empty) placeholder system, or to add new failure models etc
* Perform some simple validation on the merged models

Note that it is not intended as a modelling or analysis tool in itself. Models must still be initially created in appropriate modelling tools first. Similarly, some degree of further post-processing is expected to produce an actual runtime EDDI.


# Solution Projects
## ODELib
This project contains the 'data' (or model of the MVVM architecture) and performs the actual conversion process. The `ConverterModel` is the parent class of three different model types, representing an ODE model (ode::), a HiP-HOPS model (hip::), and a Dymodia state machine model (dym::), which can be found in the corresponding subdirectories. The `ConverterXtoY` classes (all children of `Converter`) convert from one model type to the other, though ODEtoHIP is only a limited prototype. To convert, instantiate the relevant converter, call `Convert`, and pass it the model to be converted. The ODELib is independent of the other projects, including the GUI, so it can be imported and used directly to convert models.

The actual conversion happens within the internal functions of the `ConverterXtoY` classes, primarily `ConverterHIPtoODE`. 

File handling is handled via serialisation for the most part. `hip` and `ode` classes have XML attributes that allow them to be directly serialised/deserialised to their respective formats. Dymodia uses JSON, so the `dym` classes instead make use of JSON.NET to perform loading. Standalone state machines in JSON format can also be loaded (see `StandaloneStateMachineImporter`).

Notes:
* Some ODE members are currently missing. For example, Hazards do not include Measures and Ports do not include DependabilityRequirements. StateMachine also omits the TransitionMatrices and quite a few things are missing from System.

## ODEConverterConsole
For simple console-only testing of the converter.

## ODEConverter
This is the GUI project, using WPF. Originally it was designed purely for conversion, but it has since grown into a sort of editor with additional features (hence "EDDI Editor"). It uses the Model-View-ViewModel (MVVM) architecture; the `ODELib` serves as the model (data layer), the WPF windows as the View, and the viewmodel layer is in the Viewmodels subdirectory, which echoes the structure of `ODELib`. A ViewModel object acts as an interface layer between the data, which should be ignorant of the UI, and the UI itself. This facilitates e.g. conversion of data to a more display-ready form or error checking of user input without adding UI-specific code to the data layer.

Windows include:
* **AddAction.xaml**: a small dialog for adding new Actions to States and Causes.
* **AddEvent.xaml**: a small dialog for adding new Events to Failures.
* **App.xaml**: this is just the top-level application.
* **ImportModel.xaml**: this window opens when importing (rather than opening) a model. Designed to let you view/edit a model before completing the import. Click 'convert' to convert it to ODE (if not already an ODE model) and import it. Remember to change file type if opening Dymodia files.
* **ImportSystem.xaml**: a simple prompt that lets you select a file to import (or two files, if HiP-HOPS). Not used.
* **MainWindow.xaml**: the main interface. Allows you to open/import/edit/merge ODE (EDDI) models.
* **MainWindowOld.xaml**: the old converter interface. The idea was that you import a non-ODE model (e.g. from HiP-HOPS) in one tab, then convert it and view/edit it in the second tab. No longer used, but kept just in case.

Menu commands in the main window:
* **New**: creates an empty ODE model.
* **Open**: opens an existing ODE model (replacing the current model, if any).
* **Import**: opens a new dialog that allows you to import a model, tweak it, and convert it to ODE. You can also open an existing ODE model this way and import it directly (no conversion), though it offers no benefits to do so.
* **Save**: saves the model to the current file (or opens Save As if no current filename).
* **Save As**: enter a new filename and save the model to it.
* **Export As**: in progress, but in theory will allow export to other model types (e.g. convert ODE to HiP-HOPS).
* **Exit**: ...exits.
* **Validate model** (unfinished): performs a limited validation of the model to check for e.g. disconnected causes, triggers, and deviations etc.

While in the main window, you can also right-click on any ODE model, system, or failure model to open up another import menu:
* **Import model as subsystem**: opens the import dialog and lets you import a model as a new subsytem under the current (right-clicked) system.
* **Import model and replace this subsystem**: opens the import dialog and lets you import a model to replace the current (right-clicked) system. Used for replacing placeholders/dummy systems with full ones.
* **Import new failure model**: imports a new failure model and adds it to the current system.

Models imported this way can be existing ODE models or models from another tool (which will get converted). Thus you can e.g. open a high-level ODE model from one tool, then add subsystems or replace empty subsystems with e.g. HiP-HOPS models, or import state machines from Dymodia etc.

You can also right-click on any State to add Actions to the entry/exit action lists, or Causes to add generic Actions. This opens up a small dialog to set some basic info for each Action; further (type-specific) information can be added via the properties information in the main window.

Similarly, you can right-click Failures to add Events. A similar dialog opens up to set the name, description, and type. Further information is added via the properties pane.


# Test models
A small selection of pre-made model files are included in the Example Models directory for testing and demonstration purposes:
* **Standby Recovery example** is a simple HiP-HOPS example consisting of a standby-recovery block, making it suitable as a state-based example.
	* hip_standbyRecovery_results.xml: the HiP-HOPS results file for the SR model.
	* hip_standbyRecoveryEmptyStandby_analysis.xml: this is the HiP-HOPS architecture file for a model with a placeholder standby subsystem.
	* hip_standbyRecoveryFull_analysis.xml: this is the full HiP-HOPS architecture file for the SR model (no placeholders).
	* hip_standbyRecoveryOnlyStandby_analysis.xml: this is the HiP-HOPS architecture file for just a standby subsystem. The idea is to import into the EmptyStandby model and replace the placeholder.
	* dym_primaryStandby.usm: the standalone state machine model from Dymodia (for inspection purposes).
	* dym_primaryStandbySM.uproj: the Dymodia project file containing the state machine model.
	* ode_standbyRecoveryFull.xml: the ODE exported version of the full model (including results).
	* ode_standbyRecoveryFullWithSM.xml: the ODE exported version of the full model (including results) with the imported state machine.
	* ode_standbyRecoveryEmptyyStandby.xml: the ODE exported version of the SR system with a placeholder/dummy standby subsystem (and no results).
	* ode_standbyRecoveryOnlyStandby.xml: the ODE exported version of just the standby subsystem (no results).
	* SESAME_DDI_Example.ddi: A small example in DDI format from safeTbox.
	* standalone_sm.json: A standalone JSON state machine.

