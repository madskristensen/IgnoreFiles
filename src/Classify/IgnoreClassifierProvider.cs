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

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            ITextDocument document;

            if (TextDocumentFactoryService.TryGetTextDocument(buffer, out document))
            {
                var classifier = buffer.Properties.GetOrCreateSingletonProperty(() => new IgnoreClassifier(Registry, buffer, document.FilePath));

                document.FileActionOccurred += (s, e) => {
                    if (e.FileActionType == FileActionTypes.ContentSavedToDisk)
                        classifier.Reset();
                };

                return classifier;
            }

            return null;
        }
    }
}