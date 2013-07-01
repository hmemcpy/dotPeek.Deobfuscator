using JetBrains.ActionManagement;
using JetBrains.Application.PluginSupport;

[assembly: ActionsXml("dotPeek.Deobfuscator.Actions.xml")]

// The following information is displayed in the Plugins dialog
[assembly: PluginTitle("Deobfuscator plugin for dotPeek")]
[assembly: PluginDescription("A plugin for JetBrains dotPeek Decompiler to deobfuscate assemblies, using de4dot deobfuscator.\r\n"+
                             "See LICENSES directory for additional licenses and terms.")]
[assembly: PluginVendor("Igal Tabachnik")]