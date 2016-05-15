using System;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Imaging;

namespace IgnoreFiles.Models
{
    public class IgnoreTreeModel : BindableBase
    {
        private bool _showAllFiles;
        private string _searchText;
        private string _pattern;

        public IgnoreTreeModel(string rootDirectory, string pattern)
        {
            _pattern = pattern;
            TreeRoot = FileTree.ForDirectory(rootDirectory);
            MatchesFilter = CheckMatch;
            IsSearchMatch = f => !string.IsNullOrWhiteSpace(SearchText) && f.FullPath.IndexOf(SearchText, StringComparison.OrdinalIgnoreCase) > -1;

            FileCount = TreeRoot.AllFiles.Count(x => x.IsFile && MatchesFilter(x));
            SearchIcon = WpfUtil.GetIconForImageMoniker(KnownMonikers.Search, 16, 16);
            ShowAllFilesIcon = WpfUtil.GetIconForImageMoniker(KnownMonikers.ShowAllFiles, 16, 16);
        }

        private bool CheckMatch(FileTreeModel f)
        {
            if (Helpers.CheckGlobbing(f.FullPath, _pattern) || f.Children.Any(MatchesFilter))
            {
                return true;
            }

            FileTreeModel parent = f.Parent;
            while (parent != null)
            {
                if (Helpers.CheckGlobbing(parent.FullPath, _pattern))
                {
                    return true;
                }

                parent = parent.Parent;
            }

            return false;
        }

        public FileTree TreeRoot { get; }

        public Func<FileTreeModel, bool> MatchesFilter { get; }

        public bool ShowAllFiles
        {
            get { return _showAllFiles; }
            set { Set(ref _showAllFiles, value); }
        }

        public int FileCount { get; }

        public string SearchText
        {
            get { return _searchText; }
            set { Set(ref _searchText, value, StringComparer.Ordinal); }
        }

        public ImageSource SearchIcon { get; }

        public ImageSource ShowAllFilesIcon { get; }

        public Func<FileTreeModel, bool> IsSearchMatch { get; }
    }
}
