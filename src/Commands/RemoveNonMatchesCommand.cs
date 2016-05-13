using System;
using System.ComponentModel.Design;
using System.Linq;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;

namespace IgnoreFiles
{
    internal sealed class RemoveNonMatchesCommand
    {
        private readonly Package _package;
        private ITextBuffer _buffer;

        private RemoveNonMatchesCommand(Package package)
        {
            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var cmdID = new CommandID(PackageGuids.guidPackageCmdSet, PackageIds.RemoveNonMatches);
                var command = new OleMenuCommand(Execute, cmdID);
                command.BeforeQueryStatus += BeforeQueryStatus;
                commandService.AddCommand(command);
            }
        }

        public static RemoveNonMatchesCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get { return _package; }
        }

        public static void Initialize(Package package)
        {
            Instance = new RemoveNonMatchesCommand(package);
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

            try
            {
                classifier.IsAsynchronous = false;

                using (var edit = _buffer.CreateEdit())
                {
                    var lines = _buffer.CurrentSnapshot.Lines.Reverse();

                    foreach (var line in lines)
                    {
                        var span = new SnapshotSpan(line.Start, line.LengthIncludingLineBreak);
                        var tags = classifier.GetClassificationSpans(span);

                        if (tags.Any(t => t.ClassificationType.IsOfType(IgnoreClassificationTypes.PathNoMatch)))
                        {
                            edit.Delete(span.Span);
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
                classifier.IsAsynchronous = true;
            }
        }
    }
}
