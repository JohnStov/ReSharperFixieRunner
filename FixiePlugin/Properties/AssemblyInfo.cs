using System.Reflection;
using FixiePlugin;
using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle("FixiePlugin")]
[assembly: AssemblyDescription("A Unit Test plugin for the Fixie test framework")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("John Stovin")]
[assembly: AssemblyProduct("FixiePlugin")]
[assembly: AssemblyCopyright("Copyright � 2013-2014 John Stovin")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]

[assembly: ActionsXml("FixiePlugin.Actions.xml")]

// The following information is displayed by ReSharper in the Plugins dialog
[assembly: PluginTitle("FixiePlugin")]
[assembly: PluginDescription("A Unit Test plugin for the Fixie test framework. Built using " + ReSharperInfo.ReSharperVersion + " and " + FixieInfo.FixieVersion)]
[assembly: PluginVendor("John Stovin")]
