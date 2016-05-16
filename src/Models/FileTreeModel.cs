using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace IgnoreFiles.Models
{
    public class FileTreeModel : BindableBase
    {
        private bool _isExpanded;
        private string _name;
        private List<FileTreeModel> _children;

        public FileTreeModel(string fullPath, FileTree root, bool isFile)
        {
            IsExpanded = true;
            _children = new List<FileTreeModel>();
            Root = root;
            Name = Path.GetFileName(fullPath);
            IsFile = isFile;
            FullPath = fullPath;
            int lastSlash = fullPath.LastIndexOfAny(new[] { '/', '\\' });

            //Normally this would be -1, but if the path starts with / or \, we don't want to make an empty entry
            if(lastSlash > 0)
            {
                string parentFullPath = fullPath.Substring(0, lastSlash).Trim('/', '\\');

                if(!string.IsNullOrEmpty(parentFullPath))
                {
                    Parent = root.GetModelFor(parentFullPath, false);
                    Parent?.Children?.Add(this);
                }
            }
        }

        public void ItemDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (IsFile)
            {
                IgnorePackage.DTE.ItemOperations.OpenFile(FullPath);
            }
        }

        public ImageSource CachedIcon { get; set; }

        public List<FileTreeModel> Children => _children;

        public bool IsFile { get; }

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value, StringComparer.Ordinal); }
        }

        public FileTreeModel Parent { get; }

        public FileTree Root { get; }

        public string FullPath { get; }

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { Set(ref _isExpanded, value); }
        }

        public void SortChildren()
        {
            _children = _children.OrderBy(x => x.IsFile).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }
    }
}
