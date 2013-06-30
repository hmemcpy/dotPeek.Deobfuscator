using System;
using System.Collections.Generic;
using System.Linq;
using de4dot.code;
using de4dot.code.AssemblyClient;
using de4dot.code.deobfuscators;
using de4dot.code.renamer;
using dnlib.DotNet;
using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;

namespace dotPeek.Deobfuscator
{
    public interface IAssemblyDeobfuscatorManager
    {
        IEnumerable<IDeobfuscatorInfo> KnownDeobfuscators { get; }
        FileSystemPath Execute(IAssemblyFile existingAssemblyFile, string newFileName, IProgressIndicator progressIndicator);
    }

    [ShellComponent]
    public class AssemblyDeobfuscatorManager : IAssemblyDeobfuscatorManager
    {
        private static readonly IEnumerable<IDeobfuscatorInfo> knownDeobfuscators = new IDeobfuscatorInfo[]
        {
            new de4dot.code.deobfuscators.Unknown.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Agile_NET.DeobfuscatorInfo(), // aka CliSecure
            new de4dot.code.deobfuscators.Babel_NET.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.CodeFort.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.CodeVeil.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.CodeWall.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.CryptoObfuscator.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.DeepSea.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Dotfuscator.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.dotNET_Reactor.v3.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.dotNET_Reactor.v4.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Eazfuscator_NET.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Goliath_NET.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.ILProtector.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.MaxtoCode.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.MPRESS.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Rummage.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Skater_NET.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.SmartAssembly.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Spices_Net.DeobfuscatorInfo(),
            new de4dot.code.deobfuscators.Xenocode.DeobfuscatorInfo(),
        };

        public IEnumerable<IDeobfuscatorInfo> KnownDeobfuscators
        {
            get { return knownDeobfuscators; }
        }

        public FileSystemPath Execute(IAssemblyFile existingAssemblyFile, string newFileName, IProgressIndicator progressIndicator)
        {
            var context = new ModuleContext();
            var fileOptions = new ObfuscatedFile.Options
            {
                Filename = existingAssemblyFile.Location.FullPath,
                NewFilename = newFileName,
            };

            IObfuscatedFile obfuscationFile = CreateObfuscationFile(fileOptions, context);

            return Deobfuscate(obfuscationFile, progressIndicator);
        }

        private FileSystemPath Deobfuscate(IObfuscatedFile obfuscatedFile, IProgressIndicator progressIndicator)
        {
            try
            {
                progressIndicator.Start(2);
                progressIndicator.TaskName = "Deobfuscating...";
                progressIndicator.CurrentItemText = string.Format("Saving to {0}", obfuscatedFile.NewFilename);

                obfuscatedFile.deobfuscateBegin();

                obfuscatedFile.deobfuscate();

                obfuscatedFile.deobfuscateEnd();

                progressIndicator.Advance(1);
                // turn all flags on
                RenamerFlags flags = RenamerFlags.DontCreateNewParamDefs | RenamerFlags.DontRenameDelegateFields |
                                     RenamerFlags.RenameEvents |
                                     RenamerFlags.RenameFields | RenamerFlags.RenameGenericParams |
                                     RenamerFlags.RenameMethodArgs |
                                     RenamerFlags.RenameMethods | RenamerFlags.RenameNamespaces |
                                     RenamerFlags.RenameProperties |
                                     RenamerFlags.RenameTypes | RenamerFlags.RestoreEvents |
                                     RenamerFlags.RestoreEventsFromNames |
                                     RenamerFlags.RestoreProperties | RenamerFlags.RestorePropertiesFromNames;
                var renamer = new Renamer(obfuscatedFile.DeobfuscatorContext, new[] { obfuscatedFile }, flags);
                renamer.rename();

                progressIndicator.Advance(1);
                obfuscatedFile.save();
            }
            finally
            {
                progressIndicator.Stop();
            }

            return new FileSystemPath(obfuscatedFile.NewFilename);
        }

        private IObfuscatedFile CreateObfuscationFile(ObfuscatedFile.Options fileOptions, ModuleContext moduleContext)
        {
            var obfuscatedFile = new ObfuscatedFile(fileOptions, moduleContext, new NewAppDomainAssemblyClientFactory())
            {
                DeobfuscatorContext = new DeobfuscatorContext()
            };

            try
            {
                obfuscatedFile.load(knownDeobfuscators.Select(info => info.createDeobfuscator()).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.ShowError(ex.Message);
                return null;
            }
            return obfuscatedFile;
        }
    }
}