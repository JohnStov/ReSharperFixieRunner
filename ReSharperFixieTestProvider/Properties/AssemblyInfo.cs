using System.Reflection;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("ReSharperFixieTestProvider")]
[assembly: AssemblyDescription("A Unit Test plugin for the Fixie test framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("John Stovin")]
[assembly: AssemblyProduct("ReSharperFixieTestProvider")]
[assembly: AssemblyCopyright("Copyright © John Stovin, 2013")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]

[assembly: ActionsXml("ReSharperFixieRunner.Actions.xml")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("ReSharperFixieRunner")]
[assembly: PluginDescription("A Unit Test plugin for the Fixie test framework")]
[assembly: PluginVendor("John Stovin")]
