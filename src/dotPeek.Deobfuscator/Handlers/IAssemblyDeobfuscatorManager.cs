using System.Collections.Generic;
using de4dot.code.deobfuscators;
using JetBrains.Application.Progress;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.Util;

namespace dotPeek.Deobfuscator.Handlers
{
    public interface IAssemblyDeobfuscatorManager
    {
        IEnumerable<IDeobfuscatorInfo> KnownDeobfuscators { get; }
        FileSystemPath Execute(IAssemblyFile existingAssemblyFile, string newFileName, IProgressIndicator progressIndicator);
    }
}