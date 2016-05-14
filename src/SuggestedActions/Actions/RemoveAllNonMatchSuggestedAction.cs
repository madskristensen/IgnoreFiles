using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace IgnoreFiles
{
    class RemoveAllNonMatchSuggestedAction : BaseSuggestedAction
    {
        private SnapshotSpan _span;

        public RemoveAllNonMatchSuggestedAction(SnapshotSpan span)
        {
            _span = span;
        }

        public override string DisplayText
        {
            get { return "Remove all non-matching entries"; }
        }

        public override ImageMoniker IconMoniker
        {
            get { return KnownMonikers.RecursivelyUncheckAll; }
        }

        public override void Invoke(CancellationToken cancellationToken)
        {
            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
            dte.Commands.Raise(PackageGuids.guidPackageCmdSetString, PackageIds.RemoveNonMatches, null, null);
        }
    }
}
