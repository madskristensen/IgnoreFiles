using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace IgnoreFiles
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(Options), "Text Editor\\.ignore", "General", 101, 111, true, new[] { ".gitignore", ".tfignore" }, ProvidesLocalizedCategoryName = false)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class IgnorePackage : Package
    {
        private static Options _options;

        public static Options Options
        {
            get
            {
                if (_options == null)
                {
                    // Load the package when options are needed
                    var shell = (IVsShell)GetGlobalService(typeof(SVsShell));
                    IVsPackage package;
                    shell.LoadPackage(ref PackageGuids.guidPackage, out package);
                }

                return _options;
            }
        }

        protected override void Initialize()
        {
            _options = (Options)GetDialogPage(typeof(Options));

            RemoveNonMatchesCommand.Initialize(this);
            base.Initialize();
        }
    }
}
