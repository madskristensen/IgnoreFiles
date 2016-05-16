using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IgnoreFiles.Models
{
    public class FileTree
    {
        private readonly string _rootDir;
        private List<FileTreeModel> _children = new List<FileTreeModel>();
        private readonly Dictionary<string, FileTreeModel> _lookup = new Dictionary<string, FileTreeModel>();

        public static IReadOnlyList<string> DirectoryIgnoreList { get; } = new List<string>
        {
            ".git"
        };

        public FileTree(string rootDirectory)
        {
            _rootDir = rootDirectory;
        }

        public IEnumerable<FileTreeModel> AllFiles => _lookup.Values;

        public IReadOnlyList<FileTreeModel> Children => _children;

        public static FileTree ForDirectory(string rootDirectory)
        {
            FileTree root = new FileTree(rootDirectory);

            foreach (string file in Directory.EnumerateFileSystemEntries(rootDirectory, "*", SearchOption.AllDirectories))
            {
                string[] parts = file.Split('/', '\\');
                bool skip = false;
                bool isFile = File.Exists(file);
                int fileNameParts = isFile ? 1 : 0;

                for (int i = 1; !skip && i < parts.Length - fileNameParts; ++i)
                {
                    if (DirectoryIgnoreList.Contains(parts[i], StringComparer.OrdinalIgnoreCase))
                    {
                        skip = true;
                    }
                }

                if (!skip)
                {
                    root.GetModelFor(file, isFile);
                }
            }

            root.SortChildren();
            return root;
        }

        private void SortChildren()
        {
            _children = _children.OrderBy(x => x.IsFile).ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList();

            foreach (FileTreeModel child in _children.Where(x => !x.IsFile))
            {
                child.SortChildren();
            }
        }

        public FileTreeModel GetModelFor(string fullPath, bool isFile)
        {
            if (fullPath.Length <= _rootDir.Length)
            {
                return null;
            }

            FileTreeModel existingModel;
            if (!_lookup.TryGetValue(fullPath, out existingModel))
            {
                existingModel = new FileTreeModel(fullPath, this, isFile);
                _lookup[fullPath] = existingModel;

                if (existingModel.Parent == null)
                {
                    _children.Add(existingModel);
                }
            }

            return existingModel;
        }
    }
}