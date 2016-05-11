using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
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

        [Import]
        ITextDocumentFactoryService TextDocumentFactoryService = null;

        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer buffer)
        {
            ITextDocument document;

            if (TextDocumentFactoryService.TryGetTextDocument(buffer, out document))
            {
                return buffer.Properties.GetOrCreateSingletonProperty(() => new IgnoreQuickInfo(buffer, _classifierService, document));
            }

            return null;
        }
    }
}
