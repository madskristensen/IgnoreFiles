using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Minimatch;

namespace IgnoreFiles
{
    class Helpers
    {
        public static ITextBuffer GetCurentTextBuffer()
        {
            return GetCurentTextView().TextBuffer;
        }

        public static IWpfTextView GetCurentTextView()
        {
            var componentModel = GetComponentModel();
            if (componentModel == null) return null;
            var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();

            return editorAdapter.GetWpfTextView(GetCurrentNativeTextView());
        }

        private static IVsTextView GetCurrentNativeTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));

            IVsTextView activeView = null;
            ErrorHandler.ThrowOnFailure(textManager.GetActiveView(1, null, out activeView));
            return activeView;
        }

        public static IEventsFilter DemandEventsFilterForCurrentNativeTextView()
        {
            IVsTextView view = GetCurrentNativeTextView();
            EventsFilter filter = new EventsFilter(view);
            IOleCommandTarget nextTarget;
            view.AddCommandFilter(filter, out nextTarget);
            return filter;
        }

        private static IComponentModel GetComponentModel()
        {
            return (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
        }

        private static readonly Minimatch.Options _options = new Minimatch.Options { AllowWindowsPaths = true, MatchBase = true };

        public static bool CheckGlobbing(string path, string pattern)
        {
            string p = pattern?.TrimEnd('/');

            if (!string.IsNullOrWhiteSpace(p))
            {
                return Minimatcher.Check(path, p, _options);
            }

            return false;
        }
    }
}
