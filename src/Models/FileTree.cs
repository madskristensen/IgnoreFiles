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

        public FileTree(string rootDirectory)
        {
            _rootDir = rootDirectory;
        }

        public IEnumerable<FileTreeModel> AllFiles => _lookup.Values;

        public IReadOnlyList<FileTreeModel> Children => _children;

        public static FileTree ForDirectory(string rootDirectory)
        {
            FileTree root = new FileTree(rootDirectory);

            foreach (string file in Directory.EnumerateFiles(rootDirectory, "*", SearchOption.AllDirectories))
            {
                root.GetModelFor(file, true);
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