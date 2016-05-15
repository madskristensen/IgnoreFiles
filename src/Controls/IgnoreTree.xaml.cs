using IgnoreFiles.Models;
using Microsoft.VisualStudio.Language.Intellisense;

namespace IgnoreFiles.Controls
{
    public partial class IgnoreTree : IInteractiveQuickInfoContent
    {
        public IgnoreTree()
        {
            InitializeComponent();
            this.ShouldBeThemed();
        }

        public IgnoreTree(string directory, string pattern)
            : this()
        {
            ViewModel = new IgnoreTreeModel(directory, pattern);
        }

        public IgnoreTreeModel ViewModel
        {
            get { return DataContext as IgnoreTreeModel; }
            set { DataContext = value; }
        }

        public bool KeepQuickInfoOpen => IsMouseOverAggregated || IsKeyboardFocusWithin || IsKeyboardFocused || IsFocused;

        public bool IsMouseOverAggregated => IsMouseOver || IsMouseDirectlyOver;
    }
}
