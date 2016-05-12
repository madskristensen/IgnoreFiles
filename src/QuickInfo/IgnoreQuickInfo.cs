using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

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

                    string pattern = tag.Span.GetText().Trim();
                    var files = GetFiles(_root, pattern);

                    applicableToSpan = buffer.CurrentSnapshot.CreateTrackingSpan(tag.Span.Span, SpanTrackingMode.EdgeNegative);
                    qiContent.Add(string.Join(Environment.NewLine, files.Take(_maxFiles)));

                    if (files.Count() > _maxFiles)
                    {
                        qiContent.Add($"...and {files.Count() - _maxFiles} more");
                    }
                    break;
                }
            }
        }

        public static IEnumerable<string> GetFiles(string folder, string pattern, string root = null)
        {
            var ignorePaths = IgnorePackage.Options.GetIgnorePatterns();
            var files = new List<string>();
            root = root ?? folder;

            try
            {
                foreach (var file in Directory.EnumerateFileSystemEntries(folder).Where(f => !ignorePaths.Any(p => folder.Contains(p))))
                {
                    if (pattern.EndsWith("/") && !File.GetAttributes(file).HasFlag(FileAttributes.Directory))
                        continue;

                    string relative = file.Replace(root, "").Replace("\\", "/").Trim('/');

                    if (Helpers.CheckGlobbing(relative, pattern.TrimEnd('/')))
                        files.Add(relative);
                }

                foreach (var directory in Directory.EnumerateDirectories(folder).Where(d => !ignorePaths.Any(i => d.Contains(i))))
                    files.AddRange(GetFiles(directory, pattern, root));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

            return files;
        }

        public void Dispose()
        {
            // nothing to dispose
        }
    }
}
