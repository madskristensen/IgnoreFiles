using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    [Export(typeof(IClassifierProvider))]
    [ContentType(IgnoreContentTypeDefinition.IgnoreContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    public class IgnoreClassifierProvider : IClassifierProvider
    {
        [Import]
        public IClassificationTypeRegistryService Registry { get; set; }

        [Import]
        ITextDocumentFactoryService TextDocumentFactoryService { get; set; }

        public IClassifier GetClassifier(ITextBuffer textBuffer)
        {
            ITextDocument document;
            if (TextDocumentFactoryService.TryGetTextDocument(textBuffer, out document))
            {
                return textBuffer.Properties.GetOrCreateSingletonProperty(() => new IgnoreClassifier(Registry, document.FilePath));
            }

            return null;
        }
    }
}