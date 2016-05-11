using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType(IgnoreContentTypeDefinition.IgnoreContentType)]
    [TextViewRole(PredefinedTextViewRoles.PrimaryDocument)]
    class IgnoreCreationListener : IVsTextViewCreationListener
    {
        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            if (!IgnorePackage.IsInitialized)
            {
                var shell = (IVsShell)Package.GetGlobalService(typeof(SVsShell));
                IVsPackage package;
                shell.LoadPackage(ref PackageGuids.guidPackage, out package);
            }
        }
    }
}
