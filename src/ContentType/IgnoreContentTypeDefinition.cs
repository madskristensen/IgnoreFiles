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
        [FileExtension(".tfignore")]
        public FileExtensionToContentTypeDefinition TfFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".hgignore")]
        public FileExtensionToContentTypeDefinition HgFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".nodemonignore")]
        public FileExtensionToContentTypeDefinition NodemonFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".npmignore")]
        public FileExtensionToContentTypeDefinition NpmFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".dockerignore")]
        public FileExtensionToContentTypeDefinition DockerFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".chefignore")]
        public FileExtensionToContentTypeDefinition ChefFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".cvsignore")]
        public FileExtensionToContentTypeDefinition CvsFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".boringignore")]
        public FileExtensionToContentTypeDefinition DarcsFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".mtn-ignore")]
        public FileExtensionToContentTypeDefinition MonotoneFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".p4ignore")]
        public FileExtensionToContentTypeDefinition PerforceFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".flooignore")]
        public FileExtensionToContentTypeDefinition FloobitsFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".jpmignore")]
        public FileExtensionToContentTypeDefinition JetpackFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".bzrignore")]
        public FileExtensionToContentTypeDefinition BazaarFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".jshintignore")]
        public FileExtensionToContentTypeDefinition JshintFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".eslintignore")]
        public FileExtensionToContentTypeDefinition EslintFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".cfignore")]
        public FileExtensionToContentTypeDefinition CloudFoundryFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".svnignore")]
        public FileExtensionToContentTypeDefinition SvnFileExtension { get; set; }

        [Export(typeof(FileExtensionToContentTypeDefinition))]
        [ContentType(IgnoreContentType)]
        [FileExtension(".vscodeignore")]
        public FileExtensionToContentTypeDefinition VsCodeFileExtension { get; set; }
    }
}
