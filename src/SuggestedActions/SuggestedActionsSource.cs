using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace IgnoreFiles
{
    class SuggestedActionsSource : ISuggestedActionsSource
    {
        private readonly SuggestedActionsSourceProvider m_factory;
        private readonly IgnoreClassifier _classifier;

        public SuggestedActionsSource(SuggestedActionsSourceProvider testSuggestedActionsSourceProvider, IgnoreClassifier classifier)
        {
            m_factory = testSuggestedActionsSourceProvider;
            _classifier = classifier;
        }

        public Task<bool> HasSuggestedActionsAsync(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                var spans = _classifier.GetClassificationSpans(range);

                if (spans.Any(s => s.ClassificationType.IsOfType(IgnoreClassificationTypes.Path) || s.ClassificationType.IsOfType(IgnoreClassificationTypes.PathNoMatch)))
                {
                    return true;
                }

                return false;
            });
        }

        public IEnumerable<SuggestedActionSet> GetSuggestedActions(ISuggestedActionCategorySet requestedActionCategories, SnapshotSpan range, CancellationToken cancellationToken)
        {
            var classifications = _classifier.GetClassificationSpans(range);

            foreach (var classification in classifications)
            {
                if (classification.ClassificationType.IsOfType(IgnoreClassificationTypes.Path))
                {
                    var deleteMatches = new DeleteMatchesSuggestedAction(classification.Span);
                    var excludeFromProj = new ExcludeMatchesFromProjectSuggestedAction(classification.Span);

                    return CreateActionSet(deleteMatches, excludeFromProj);
                }
                else if (classification.ClassificationType.IsOfType(IgnoreClassificationTypes.PathNoMatch))
                {
                    var removeNonMatch = new RemoveNonMatchSuggestedAction(classification.Span);
                    var removeAllNonMatch = new RemoveAllNonMatchSuggestedAction(classification.Span);

                    return CreateActionSet(removeNonMatch, removeAllNonMatch);
                }
            }

            return Enumerable.Empty<SuggestedActionSet>();
        }

        public IEnumerable<SuggestedActionSet> CreateActionSet(params BaseSuggestedAction[] actions)
        {
            var enabledActions = actions.Where(action => action.IsEnabled);
            return new[] { new SuggestedActionSet(enabledActions) };
        }

        public void Dispose()
        {
        }

        public bool TryGetTelemetryId(out Guid telemetryId)
        {
            // This is a sample provider and doesn't participate in LightBulb telemetry
            telemetryId = Guid.Empty;
            return false;
        }


        public event EventHandler<EventArgs> SuggestedActionsChanged
        {
            add { }
            remove { }
        }
    }
}
