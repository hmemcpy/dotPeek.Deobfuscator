dotPeek.Deobfuscator
====================

A plugin for [JetBrains dotPeek Decompiler](http://www.jetbrains.com/decompiler/) to deobfuscate assemblies, using the excellent [de4dot deobfuscator](https://bitbucket.org/0xd4d/de4dot/).


## How does it work?

The plugin is a wrapper that calls de4dot to perform the heavy lifting! For list of supported obfuscators, please visit the [de4dot site](https://bitbucket.org/0xd4d/de4dot/).

From dotPeek, select the assembly you'd like to deobfuscate, and press the **Deobfuscate** button on the menu bar, or right-click the assembly.

![](http://i.imgur.com/lGDzTMO.png)

## Installation

If you wish to just install a copy of the plugins without building yourself:

- Download the latest zip file: [**dotPeek.Deobfuscator.zip**](..)
- Extract everything
- Run the batch file Install-dotPeek.Deobfuscator.1.1.bat for dotPeek v1.1.

## License

The plugin and the components it uses (mainly, de4dot) are licensed under GNU General Public License v3 (GPLv3).
Please see the file COPYING for additional terms and conditions.

## Building

To build the source, you need the [ReSharper SDK](http://www.jetbrains.com/resharper/download/index.html) installed.

In addition, you will need to configure de4dot for building. Please refer to the [de4dot wiki](https://bitbucket.org/0xd4d/de4dot/wiki).


## Contributing

Feel free to [raise issues](https://github.com/hmemcpy/dotPeek.Deobfuscator/issues), or [fork the project](http://help.github.com/fork-a-repo/) and [send a pull request](http://help.github.com/send-pull-requests/).

