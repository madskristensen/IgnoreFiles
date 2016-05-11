using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

namespace IgnoreFiles
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidPackageString)]
    public sealed class IgnorePackage : Package
    {
        public static bool IsInitialized { get; private set; }

        protected override void Initialize()
        {
            IsInitialized = true;
            RemoveNonMatchesCommand.Initialize(this);
            base.Initialize();
        }
    }
}
