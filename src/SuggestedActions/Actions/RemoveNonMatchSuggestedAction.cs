using System.Threading;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text;

namespace IgnoreFiles
{
    class RemoveNonMatchSuggestedAction : BaseSuggestedAction
    {
        private SnapshotSpan _span;

        public RemoveNonMatchSuggestedAction(SnapshotSpan span)
        {
            _span = span;
        }

        public override string DisplayText
        {
            get { return "Remove non-matching entry"; }
        }

        public override void Invoke(CancellationToken cancellationToken)
        {
            var line = _span.Snapshot.GetLineFromPosition(_span.Start.Position);

            using (var edit = _span.Snapshot.TextBuffer.CreateEdit())
            {
                edit.Delete(line.Start.Position, line.LengthIncludingLineBreak);
                edit.Apply();
            }
        }
    }
}
