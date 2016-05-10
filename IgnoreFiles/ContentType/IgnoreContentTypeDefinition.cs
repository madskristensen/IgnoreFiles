using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace IgnoreFiles
{
    public class IgnoreContentTypeDefinition
    {
        public const string IgnoreContentType = "Ignore";

        [Export(typeof(ContentTypeDefinition))]
        [Name(IgnoreContentType)]
        [BaseDefinition("plaintext")]
        public ContentTypeDefinition IIgnoreContentType { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".gitignore")]
        public FileExtensionToContentTypeDefinition GitFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".hgignore")]
        public FileExtensionToContentTypeDefinition HgFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".eslintegnore")]
        public FileExtensionToContentTypeDefinition EslintFileExtension { get; set; }
    }
}
