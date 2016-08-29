using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace IgnoreFiles
{
    internal sealed class RemoveNonMatchesCommand
    {
        private ITextBuffer _buffer;
        private DTE2 _dte;

        private RemoveNonMatchesCommand(OleMenuCommandService commandService, DTE2 dte)
        {
            _dte = dte;

            var cmdID = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.RemoveNonMatches);
            var command = new OleMenuCommand(Execute, cmdID);
            command.BeforeQueryStatus += BeforeQueryStatus;
            commandService.AddCommand(command);
        }

        public static RemoveNonMatchesCommand Instance
        {
            get;
            private set;
        }

        public static async System.Threading.Tasks.Task Initialize(AsyncPackage package)
        {
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Instance = new RemoveNonMatchesCommand(commandService, dte);
        }

        private void BeforeQueryStatus(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            button.Enabled = button.Visible = false;

            _buffer = Helpers.GetCurentTextBuffer();

            if (_buffer != null && _buffer.ContentType.IsOfType(IgnoreContentTypeDefinition.IgnoreContentType))
            {
                button.Enabled = button.Visible = true;
            }
        }

        private void Execute(object sender, EventArgs e)
        {
            IgnoreClassifier classifier;

            if (!_buffer.Properties.TryGetProperty(typeof(IgnoreClassifier), out classifier))
                return;

            int linesRemoved = 0;

            try
            {
                _dte.StatusBar.Text = "Analyzing file and removing non-matches...";
                _dte.UndoContext.Open("Removed non-matches");

                using (var edit = _buffer.CreateEdit())
                {
                    var lines = _buffer.CurrentSnapshot.Lines.Reverse();

                    foreach (var line in lines)
                    {
                        var span = new SnapshotSpan(line.Start, line.LengthIncludingLineBreak);

                        if (classifier.HasMatches(span))
                        {
                            edit.Delete(span.Span);
                            linesRemoved += 1;
                        }
                    }

                    edit.Apply();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            finally
            {
                _dte.StatusBar.Text = $"{linesRemoved} non-matching entries removed";
                _dte.UndoContext.Close();
            }
        }
    }
}
