using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Media;
using EnvDTE;

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
            if (!IsFile)
            {
                return;
            }

            try
            {
                for (int i = 1; i < IgnorePackage.DTE.Windows.Count; ++i)
                {
                    try
                    {
                        Window window = IgnorePackage.DTE.Windows.Item(i);
                        Document d = window.Document;

                        if (string.Equals(d?.Path + d?.Name, FullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            window.Activate();
                            window.Visible = true;
                            return;
                        }
                    }
                    catch
                    {
                    }
                }

                Window win = IgnorePackage.DTE.OpenFile(null, FullPath);
                if (win != null)
                {
                    win.Activate();
                    win.Visible = true;

                }
            }
            catch
            {
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
