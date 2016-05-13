using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace IgnoreFiles
{
    public class IgnoreClassifier : IClassifier
    {
        private IClassificationType _symbol, _comment, _path, _pathNoMatch;
        private static Regex _commentRegex = new Regex(@"(?<!\\)(#.+)", RegexOptions.Compiled);
        private static Regex _pathRegex = new Regex(@"(?<path>^[^:#\r\n]+)", RegexOptions.Compiled);
        private static Regex _symbolRegex = new Regex(@"^(?<name>syntax)(?::[^#:]+)", RegexOptions.Compiled);
        private ConcurrentDictionary<string, bool> _cache = new ConcurrentDictionary<string, bool>();
        private Queue<Tuple<string, SnapshotSpan>> _queue = new Queue<Tuple<string, SnapshotSpan>>();
        private string _root;
        private ITextBuffer _buffer;
        private bool _isResetting;
        private Timer _timer;

        public IgnoreClassifier(IClassificationTypeRegistryService registry, ITextBuffer buffer, string fileName)
        {
            _buffer = buffer;
            _root = Path.GetDirectoryName(fileName);
            _comment = registry.GetClassificationType(PredefinedClassificationTypeNames.Comment);
            _path = registry.GetClassificationType(IgnoreClassificationTypes.Path);
            _pathNoMatch = registry.GetClassificationType(IgnoreClassificationTypes.PathNoMatch);
            _symbol = registry.GetClassificationType(IgnoreClassificationTypes.Keyword);

            _timer = new Timer(250);
            _timer.Elapsed += TimerElapsed;
        }

        public bool IsAsynchronous { get; set; } = true;

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

                // Whole line is a comment, so just return here
                if (comment.Index == 0)
                    return list;
            }

            var symbolMatch = _symbolRegex.Match(text);
            if (symbolMatch.Success)
            {
                var keyword = GetSpan(span, symbolMatch.Groups["name"], _symbol);
                list.Add(keyword);

                // Whole line is a symbol, so just return here
                return list;
            }

            var pathMatch = _pathRegex.Match(text);

            if (!pathMatch.Success)
                return list;

            var pathType = GetPathClassificationType(pathMatch.Groups["path"].Value.Trim(), span);

            var path = GetSpan(span, pathMatch.Groups["path"], pathType);
            if (path != null)
                list.Add(path);

            return list;
        }

        public void Reset()
        {
            if (!_isResetting)
            {
                _isResetting = true;
                _cache.Clear();
                _queue.Clear();
                var span = new SnapshotSpan(_buffer.CurrentSnapshot, 0, _buffer.CurrentSnapshot.Length);
                OnClassificationChanged(span);
                _isResetting = false;
            }
        }

        private IClassificationType GetPathClassificationType(string pattern, SnapshotSpan span)
        {
            if (pattern.StartsWith("../"))
                return _pathNoMatch;

            if (!_cache.ContainsKey(pattern))
            {
                if (IsAsynchronous)
                {
                    _queue.Enqueue(Tuple.Create(pattern, span));
                    _timer.Start();
                }
                else
                {
                    ProcessPath(pattern, span);
                    return GetPathClassificationType(pattern, span);
                }

                return _path;
            }

            return _cache[pattern] ? _path : _pathNoMatch;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (_queue.Count == 0)
                return;

            _timer.Stop();

            Task.Run(() =>
            {
                try
                {
                    do
                    {
                        var t = _queue.Dequeue();

                        if (_buffer.CurrentSnapshot.Version == t.Item2.Snapshot.Version)
                            ProcessPath(t.Item1, t.Item2);

                    } while (_queue.Count > 0);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            });
        }

        private void ProcessPath(string pattern, SnapshotSpan span)
        {
            bool hasFiles = IgnoreQuickInfo.GetFiles(_root, pattern).Any();

            _cache[pattern] = hasFiles;

            if (!hasFiles)
            {
                OnClassificationChanged(span);
            }
        }

        private void OnClassificationChanged(SnapshotSpan span)
        {
            if (_buffer.CurrentSnapshot.Version == span.Snapshot.Version)
                ClassificationChanged?.Invoke(this, new ClassificationChangedEventArgs(span));
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

        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
    }
}