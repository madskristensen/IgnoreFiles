using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace IgnoreFiles
{
    internal class IgnoreQuickInfo : IQuickInfoSource
    {
        IClassifier _classifier;

        public IgnoreQuickInfo(ITextBuffer buffer, IClassifierAggregatorService classifier)
        {
            _classifier = classifier.GetClassifier(buffer);
        }

        public void AugmentQuickInfoSession(IQuickInfoSession session, IList<object> qiContent, out ITrackingSpan applicableToSpan)
        {
            applicableToSpan = null;
            var buffer = session.TextView.TextBuffer;

            SnapshotPoint? point = session.GetTriggerPoint(buffer.CurrentSnapshot);

            if (!point.HasValue)
                return;

            int position = point.Value.Position;
            var line = point.Value.GetContainingLine();
            var span = new SnapshotSpan(line.Start, line.Length);
            var tags = _classifier.GetClassificationSpans(span);

            foreach (var tag in tags.Where(t => t.Span.Contains(position)))
            {
                if (tag.ClassificationType.IsOfType(IgnoreClassificationTypes.PathNoMatch))
                {
                    string text = tag.Span.GetText();
                    applicableToSpan = buffer.CurrentSnapshot.CreateTrackingSpan(tag.Span.Span, SpanTrackingMode.EdgeNegative);
                    qiContent.Add($"The path \"{text}\" does not point to any existing file");
                    break;
                }
            }
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
