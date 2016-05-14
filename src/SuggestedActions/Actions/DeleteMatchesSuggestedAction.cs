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
    class DeleteMatchesSuggestedAction : BaseSuggestedAction
    {
        private SnapshotSpan _span;

        public DeleteMatchesSuggestedAction(SnapshotSpan span)
        {
            _span = span;
        }

        public override string DisplayText
        {
            get { return "Delete matching file entries..."; }
        }

        public override ImageMoniker IconMoniker
        {
            get { return KnownMonikers.DeleteDocument; }
        }

        public override void Invoke(CancellationToken cancellationToken)
        {
            var okToProceed = MessageBox.Show("Are you sure you want to delete the mathing file system entries?", Vsix.Name, MessageBoxButton.OKCancel, MessageBoxImage.Question);
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
                        item.Delete();
                    }
                    else
                    {
                        if (Directory.Exists(entry))
                            Directory.Delete(entry, true);
                        else if (File.Exists(entry))
                            File.Delete(entry);
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
