using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace IgnoreFiles
{
    public class IgnoreClassifier : IClassifier
    {
        private IClassificationType _symbol, _comment, _path, _operator;
        private Regex _commentRegex = new Regex(@"(?<!\\)(#.+)", RegexOptions.Compiled);
        private Regex _pathRegex = new Regex(@"(?<path>^[^:#\r\n]+)", RegexOptions.Compiled);
        private Regex _operatorRegex = new Regex(@"(?<!\\)\[([0-9-]+)\]", RegexOptions.Compiled);
        private Regex _symbolRegex = new Regex(@"^(?<name>syntax)(?::[^#:]+)", RegexOptions.Compiled);

        public IgnoreClassifier(IClassificationTypeRegistryService registry)
        {
            _comment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            _path = registry.GetClassificationType(IgnoreClassificationTypes.Path);
            _operator = registry.GetClassificationType(IgnoreClassificationTypes.Operator);
            _symbol = registry.GetClassificationType(IgnoreClassificationTypes.Keyword);
        }

        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            IList<ClassificationSpan> list = new List<ClassificationSpan>();

            string text = span.GetText();

            if (string.IsNullOrWhiteSpace(text))
                return list;

            var comment = _commentRegex.Match(text);

            if (comment.Success)
            {
                var result = new SnapshotSpan(span.Snapshot, span.Start + comment.Index, comment.Length);
                list.Add(new ClassificationSpan(result, _comment));

                // Whole line is a comment
                if (comment.Index == 0)
                    return list;
            }

            var symbolMatch = _symbolRegex.Match(text);
            if (symbolMatch.Success)
            {
                var keyword = GetSpan(span, symbolMatch.Groups["name"], _symbol);
                list.Add(keyword);

                return list;
            }

            var pathMatch = _pathRegex.Match(text);

            if (!pathMatch.Success)
                return list;

            var path = GetSpan(span, pathMatch.Groups["path"], _path);
            if (path != null)
            {
                foreach (Match opMatch in _operatorRegex.Matches(text))
                {
                    list.Add(GetSpan(span, opMatch.Groups[0], _operator));
                }

                list.Add(path);
            }

            return list;
        }

        private ClassificationSpan GetSpan(SnapshotSpan span, Group group, IClassificationType type)
        {
            if (group.Length > 0)
            {
                var result = new SnapshotSpan(span.Snapshot, span.Start + group.Index, group.Length);
                return new ClassificationSpan(result, type);
            }

            return null;
        }

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged
        {
            add { }
            remove { }
        }
    }
}