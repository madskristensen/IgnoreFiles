using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    [Export(typeof(ISuggestedActionsSourceProvider))]
    [Name("Test Suggested Actions")]
    [ContentType(IgnoreContentTypeDefinition.IgnoreContentType)]
    class SuggestedActionsSourceProvider : ISuggestedActionsSourceProvider
    {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        public ISuggestedActionsSource CreateSuggestedActionsSource(ITextView textView, ITextBuffer textBuffer)
        {
            IgnoreClassifier classifier;

            if (!textBuffer.Properties.TryGetProperty(typeof(IgnoreClassifier), out classifier))
                return null;

            return textView.Properties.GetOrCreateSingletonProperty(() => new SuggestedActionsSource(this, classifier));
        }
    }
}
