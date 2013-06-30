using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.Application.DataContext;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.Application.Settings.Store.Implementation;
using JetBrains.IDE.TreeBrowser;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.Impl;
using JetBrains.ProjectModel.Model2.Assemblies.Interfaces;
using JetBrains.ProjectModel.Model2.References;
using JetBrains.ReSharper.Feature.Services.ExternalSources.CSharp.AssemblyExport;
using JetBrains.ReSharper.Features.Browsing.AssemblyExplorer;
using JetBrains.ReSharper.Features.Browsing.AssemblyExplorer.ExplorerNodesModel.Core;
using JetBrains.ReSharper.Features.Browsing.AssemblyExplorer.ExplorerNodesModel.Nodes;
using JetBrains.ReSharper.Features.Browsing.AssemblyExplorer.ProjFileModel.Export;
using JetBrains.ReSharper.Features.Browsing.AssemblyExplorer.ProjFileModel.Export.History;
using JetBrains.TreeModels;
using JetBrains.UI.Application.Progress;
using JetBrains.UI.CrossFramework;
using JetBrains.UI.Settings;
using JetBrains.Util;
using System.Collections.Generic;
using System.Linq;

namespace dotPeek.Deobfuscator
{
    [ActionHandler("dotPeek.Deobfuscator.CommandWindowAction")]
    public class CommandWindowActionHandler : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
        {
            ISolution solution;
            return TryGetExistingAssemblyFile(context, out solution) != null;
        }

        public void Execute(IDataContext context,DelegateExecute nextExecute)
        {
            ISolution solution;
            IAssemblyFile existingAssemblyFile = TryGetExistingAssemblyFile(context, out solution);
            if (existingAssemblyFile == null)
                return;

            var deobfuscator = solution.TryGetComponent<IAssemblyDeobfuscatorManager>();
            if (deobfuscator == null)
                return;
            
            string newFileName = AskUser(existingAssemblyFile);
            if (newFileName == null)
                return;

            FileSystemPath newAssembly = null;
            Shell.Instance.GetComponent<UITaskExecutor>().FreeThreaded.ExecuteTask("Deobfuscating...", TaskCancelable.Yes, progressIndicator =>
            {
                using (ReadLockCookie.Create())
                    newAssembly = deobfuscator.Execute(existingAssemblyFile, newFileName, progressIndicator);
            });

            if (newAssembly != null)
            {
                AddToAssemblyExplorer(newAssembly, solution);
            }
        }

        private string AskUser(IAssemblyFile existingAssemblyFile)
        {
            var saveFileDialog = new SaveFileDialog
            {
                OverwritePrompt = true,
                FileName = existingAssemblyFile.Location.NameWithoutExtension + "-deobfuscated" + existingAssemblyFile.Location.ExtensionWithDot,
                DefaultExt = "dll",
                Filter = "DLL File (*.dll)|*.dll|All Files (*.*)|*.*",
            };

            using (saveFileDialog)
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return null;

                return saveFileDialog.FileName;
            }
        }

        private static void AddToAssemblyExplorer(FileSystemPath assemblyPath, ISolution solution)
        {
            var component = solution.TryGetComponent<IAssemblyExplorerManager>();
            if (component == null)
                return;

            component.AddItemsByPath(new[] { assemblyPath });
        }


        private static IAssemblyFile TryGetExistingAssemblyFile(IDataContext context, out ISolution solution)
        {
            solution = context.GetData(JetBrains.ProjectModel.DataContext.DataConstants.SOLUTION);
            if (solution == null)
                return null;
            IList<TreeModelNode> data = context.GetData(TreeModelBrowser.TREE_MODEL_NODES);
            if (data == null || data.Count != 1)
                return null;
            IAssemblyFile assemblyFile = GetAssemblyFile(data.First());
            if (assemblyFile == null)
                return null;
            AssemblyInfoCache component = solution.TryGetComponent<AssemblyInfoCache>();
            if (component == null)
                return null;
            if (!assemblyFile.Location.ExistsFile || AssemblyExplorerUtil.AssemblyIsBroken(assemblyFile.Location, component))
                return null;
            
            return assemblyFile;
        }

        private static IAssemblyFile GetAssemblyFile(TreeModelNode assemblyNode)
        {
            IAssemblyFile assemblyFile = null;
            var assemblyFileNode = assemblyNode.DataValue as IAssemblyFileNode;
            if (assemblyFileNode != null)
            {
                assemblyFile = assemblyFileNode.GetAssemblyFile();
            }
            else
            {
                var assemblyReferenceNode = assemblyNode.DataValue as AssemblyReferenceNode;
                if (assemblyReferenceNode != null)
                {
                    IAssembly assemblyResolveResult = assemblyReferenceNode.Reference.GetModuleToAssemblyResolveResult();
                    if (assemblyResolveResult != null)
                        assemblyFile = assemblyResolveResult.GetFiles().FirstOrDefault();
                }
            }

            return assemblyFile;
        }
    }
}