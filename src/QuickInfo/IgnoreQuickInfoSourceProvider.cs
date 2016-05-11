using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("Ignore QuickInfo Source")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType(IgnoreContentTypeDefinition.IgnoreContentType)]
    internal class ImageHtmlQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        IClassifierAggregatorService _classifierService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty(() => new IgnoreQuickInfo(textBuffer, _classifierService));
        }
    }
}
