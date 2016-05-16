using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EnvDTE;
using IgnoreFiles.Models;
using Microsoft.VisualStudio.Language.Intellisense;

namespace IgnoreFiles.Controls
{
    public partial class IgnoreTree : IInteractiveQuickInfoContent
    {
        private readonly Action _closeAction;

        public IgnoreTree()
        {
            InitializeComponent();
            this.ShouldBeThemed();
        }

        public IgnoreTree(string directory, string pattern, Action closeAction)
            : this()
        {
            _closeAction = closeAction;
            ViewModel = new IgnoreTreeModel(directory, pattern);
        }

        public IgnoreTreeModel ViewModel
        {
            get { return Dispatcher.Invoke(() => DataContext as IgnoreTreeModel); }
            set { Dispatcher.Invoke(() => DataContext = value); }
        }

        public bool KeepQuickInfoOpen => IsMouseOverAggregated || IsKeyboardFocusWithin || IsKeyboardFocused || IsFocused;

        public bool IsMouseOverAggregated => IsMouseOver || IsMouseDirectlyOver;

        private void SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            FileTreeModel selected = e.NewValue as FileTreeModel;

            if (selected != null)
            {
                UIHierarchy solutionExplorer = (UIHierarchy) IgnorePackage.DTE.Windows.Item(Constants.vsext_wk_SProjectWindow).Object;
                UIHierarchyItem rootNode = solutionExplorer.UIHierarchyItems.Item(1);

                Stack<Tuple<UIHierarchyItems, int, bool>> parents = new Stack<Tuple<UIHierarchyItems, int, bool>>();
                ProjectItem targetItem = IgnorePackage.DTE.Solution.FindProjectItem(selected.FullPath);

                UIHierarchyItems collection = rootNode.UIHierarchyItems;
                int cursor = 1;
                bool oldExpand = collection.Expanded;

                while (cursor <= collection.Count || parents.Count > 0)
                {
                    while (cursor > collection.Count && parents.Count > 0)
                    {
                        collection.Expanded = oldExpand;
                        Tuple<UIHierarchyItems, int, bool> parent = parents.Pop();
                        collection = parent.Item1;
                        cursor = parent.Item2;
                        oldExpand = parent.Item3;
                    }

                    if (cursor > collection.Count)
                    {
                        break;
                    }

                    UIHierarchyItem result = collection.Item(cursor);
                    ProjectItem item = result.Object as ProjectItem;

                    if (item == targetItem)
                    {
                        result.Select(vsUISelectionType.vsUISelectionTypeSelect);
                        return;
                    }

                    ++cursor;

                    oldExpand = result.UIHierarchyItems.Expanded;
                    result.UIHierarchyItems.Expanded = true;
                    if (result.UIHierarchyItems.Count > 0)
                    {
                        parents.Push(Tuple.Create(collection, cursor, oldExpand));
                        collection = result.UIHierarchyItems;
                        cursor = 1;
                    }
                }
            }
        }

        private void ItemDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            FileTreeModel model = item?.DataContext as FileTreeModel;
            model?.ItemDoubleClicked(sender, e);
            _closeAction?.Invoke();
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _closeAction?.Invoke();
            }
        }
    }
}
