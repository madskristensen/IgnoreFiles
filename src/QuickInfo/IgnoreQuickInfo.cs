using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Minimatch;

namespace IgnoreFiles
{
    internal class IgnoreQuickInfo : IQuickInfoSource
    {
        private IClassifier _classifier;
        private string _root;
        private const int _maxFiles = 20;

        public IgnoreQuickInfo(ITextBuffer buffer, IClassifierAggregatorService classifier, ITextDocument document)
        {
            _classifier = classifier.GetClassifier(buffer);
            _root = Path.GetDirectoryName(document.FilePath);
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

                    // Only show error tooltips
                    if (!text.Contains("../") && !IgnorePackage.Options.ShowTooltip)
                        continue;

                    string tooltip = $"The path \"{text}\" does not point to any existing file";

                    if (text.StartsWith("../"))
                        tooltip = "The entry contains a relative path segment which is not allowed";

                    applicableToSpan = buffer.CurrentSnapshot.CreateTrackingSpan(tag.Span.Span, SpanTrackingMode.EdgeNegative);
                    qiContent.Add(tooltip);
                    break;
                }
                else if (tag.ClassificationType.IsOfType(IgnoreClassificationTypes.Path))
                {
                    if (!IgnorePackage.Options.ShowTooltip)
                        continue;

                    string text = IgnoreClassifier.CleanPattern(tag.Span.GetText());
                    var files = GetFiles(_root, text);

                    applicableToSpan = buffer.CurrentSnapshot.CreateTrackingSpan(tag.Span.Span, SpanTrackingMode.EdgeNegative);
                    qiContent.Add(string.Join(Environment.NewLine, files.Take(_maxFiles)));

                    if (files.Count > _maxFiles)
                    {
                        qiContent.Add($"...and {files.Count - _maxFiles} more");
                    }
                    break;
                }
            }
        }

        private List<string> GetFiles(string folder, string pattern)
        {
            var ignorePaths = IgnorePackage.Options.GetIgnorePatterns();
            var files = new List<string>();

            if (ignorePaths.Any(i => folder.Contains(i)))
                return files;

            try
            {
                foreach (var file in Directory.EnumerateFileSystemEntries(folder))
                {
                    string relative = file.Replace(_root, "").TrimStart('\\');
                    if (Minimatcher.Check(relative, pattern, new Minimatch.Options { AllowWindowsPaths = true }))
                        files.Add(relative);
                }

                foreach (var directory in Directory.EnumerateDirectories(folder))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch { }

            return files;
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
