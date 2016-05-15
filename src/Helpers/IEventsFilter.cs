using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace IgnoreFiles
{
    public interface IEventsFilter
    {
        void Remove();
    }

    internal class EventsFilter : IEventsFilter, IOleCommandTarget
    {
        private readonly IVsTextView _view;

        public EventsFilter(IVsTextView view)
        {
            _view = view;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return VSConstants.S_OK;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            return VSConstants.S_OK;
        }

        public void Remove()
        {
            _view.RemoveCommandFilter(this);
        }
    }
}
