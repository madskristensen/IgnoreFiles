using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    [Export(typeof(ITaggerProvider))]
    [ContentType(IgnoreContentTypeDefinition.IgnoreContentType)]
    [TagType(typeof(ErrorTag))]
    class IgnoreErrorTaggerProvider : ITaggerProvider
    {
        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag
        {
            return buffer.Properties.GetOrCreateSingletonProperty(() => new IgnoreErrorTagger() as ITagger<T>);
        }
    }
}
