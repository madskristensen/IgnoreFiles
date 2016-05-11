using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    [Export(typeof(IIntellisenseControllerProvider))]
    [Name("Ignore QuickInfo Controller")]
    [ContentType(IgnoreContentTypeDefinition.IgnoreContentType)]
    public class ImageHtmlQuickInfoControllerProvider : IIntellisenseControllerProvider
    {
        [Import]
        public IQuickInfoBroker QuickInfoBroker { get; set; }

        public IIntellisenseController TryCreateIntellisenseController(ITextView textView, IList<ITextBuffer> subjectBuffers)
        {
            return new ImageHtmlQuickInfoController(textView, subjectBuffers, this);
        }
    }
}
