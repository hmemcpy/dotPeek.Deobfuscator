using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;
using JetBrains.Util;

namespace dotPeek.Deobfuscator
{
    [ActionHandler("dotPeek.Deobfuscator.AboutAction")]
    public class AboutActionHandler : IActionHandler
    {
        public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nexUpdate)
        {
            // return true/false to enable/disable this action
            return true;
        }
        
        public void Execute(IDataContext context,DelegateExecute nextExecute)
        {
            MessageBox.ShowInfo("About information for the dotPeek.Deobfuscator plugin goes here.");
        }        
    }
}