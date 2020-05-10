using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib
{
    public class AssemblySandboxBuilder
    {
        public static IAssemblySandbox Create(Guid sandboxId, string applicationBaseName = "Application", string parentDirectory = "")
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var applicationBase = string.IsNullOrEmpty(parentDirectory)
                ? Path.Combine(baseDirectory, applicationBaseName)
                : Path.Combine(baseDirectory, parentDirectory, applicationBaseName);

            if (!Directory.Exists(applicationBase))
                Directory.CreateDirectory(applicationBase);

            var coreLibDllFile = Path.Combine(baseDirectory, "CoreLib.dll");
            if (File.Exists(coreLibDllFile))
                File.Copy(coreLibDllFile, Path.Combine(applicationBase, "CoreLib.dll"), true);

            var sharedDllFiles = Directory.GetFiles(Path.Combine(baseDirectory, "Shared"), "*.dll");
            foreach (var dllFile in sharedDllFiles)
                File.Copy(dllFile, Path.Combine(applicationBase, Path.GetFileName(dllFile)), true);

            var privateBinPath = Path.Combine(applicationBaseName, "bin");
            var shadowCopyDirectory = Path.Combine(applicationBaseName, "_shadow");

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = applicationBase,
                //PrivateBinPath = privateBinPath,
                //ShadowCopyFiles = "true",
                //ShadowCopyDirectories = shadowCopyDirectory
            };

            var appDomainPermissions = new PermissionSet(PermissionState.Unrestricted);
            var appDomain = AppDomain.CreateDomain(sandboxId.ToString(), null, appDomainSetup, appDomainPermissions, null);

            appDomain.AssemblyLoad += AppDomain_AssemblyLoad;
            //appDomain.AssemblyResolve += AppDomain_AssemblyResolve;
            //appDomain.ReflectionOnlyAssemblyResolve += AppDomain_ReflectionOnlyAssemblyResolve;

            Type assmeblySandboxType = typeof(AssemblySandbox);
            return (IAssemblySandbox)appDomain.CreateInstanceFromAndUnwrap(assmeblySandboxType.Assembly.Location, assmeblySandboxType.FullName);
        }

        private static void AppDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Console.WriteLine($"===========>  {args.LoadedAssembly.FullName}");
        }
    }
}
