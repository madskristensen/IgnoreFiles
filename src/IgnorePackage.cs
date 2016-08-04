using System;
using System.Runtime.InteropServices;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace IgnoreFiles
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Text Editor\\.ignore", "General", 101, 111, true, new[] { ".gitignore", ".tfignore" }, ProvidesLocalizedCategoryName = false)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class IgnorePackage : Package
    {
        private static Options _options;
        private static DTE2 _dte;

        public static DTE2 DTE
        {
            get
            {
                if (_dte == null)
                {
                    _dte = (DTE2)ServiceProvider.GlobalProvider.GetService(typeof(SDTE));
                }

                return _dte;
            }
        }

        public static Options Options
        {
            get
            {
                if (_options == null)
                {
                    // Load the package when options are needed
                    var shell = (IVsShell)GetGlobalService(typeof(SVsShell));
                    IVsPackage package;
                    ErrorHandler.ThrowOnFailure(shell.LoadPackage(ref PackageGuids.guidPackage, out package));
                }

                return _options;
            }
        }

        protected override void Initialize()
        {
            _options = (Options)GetDialogPage(typeof(Options));

            Logger.Initialize(this, Vsix.Name);
            RemoveNonMatchesCommand.Initialize(this);

            base.Initialize();
        }
    }
}
