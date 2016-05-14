using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace IgnoreFiles
{
    class ExcludeMatchesFromProjectSuggestedAction : BaseSuggestedAction
    {
        private SnapshotSpan _span;

        public ExcludeMatchesFromProjectSuggestedAction(SnapshotSpan span)
        {
            _span = span;
        }

        public override string DisplayText
        {
            get { return "Exclude matching entries from project..."; }
        }

        public override ImageMoniker IconMoniker
        {
            get { return KnownMonikers.HiddenFile; }
        }

        public override bool IsEnabled
        {
            get
            {
                var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
                return !string.IsNullOrEmpty(dte.Solution?.FullName);
            }
        }

        public override void Invoke(CancellationToken cancellationToken)
        {
            var okToProceed = MessageBox.Show("Are you sure you want remove the entries from the project?\r\rNo files or folders will be deleted from disk.", Vsix.Name, MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (okToProceed != MessageBoxResult.OK)
                return;

            var dte = (DTE2)Package.GetGlobalService(typeof(DTE));

            string folder = Path.GetDirectoryName(dte.ActiveDocument.FullName);
            string pattern = _span.GetText().Trim();
            var entries = IgnoreQuickInfo.GetFiles(folder, pattern);

            foreach (var entry in entries.Select(e => Path.Combine(folder, e).Replace("/", "\\")))
            {
                try
                {
                    var item = dte.Solution?.FindProjectItem(entry);

                    if (item != null)
                    {
                        item.Remove();
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }
    }
}
