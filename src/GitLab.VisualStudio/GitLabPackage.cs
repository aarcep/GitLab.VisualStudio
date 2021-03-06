﻿using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GitLab.VisualStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [Guid("54803a44-49e0-4935-bba4-7d7d91682273")]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // this is the Git service GUID, so we load whenever it loads
    [ProvideAutoLoad("11B8E6D7-C08B-4385-B321-321078CDD1F8")]
    public class GitLabPackage : Package
    {
        public GitLabPackage()
        {
            ServiceProvider = this;
        }

        public static System.IServiceProvider ServiceProvider { get; set; }

        private static DTE2 _dte;

        internal static DTE2 DTE
        {
            get
            {
                if (_dte == null && ServiceProvider!=null)
                {
                    _dte = ServiceProvider.GetService(typeof(DTE)) as DTE2;
                }

                return _dte;
            }
        }
        public static string GetActiveFilePath()
        {
            string path = "";
            if (DTE != null)
            {
                // sometimes, DTE.ActiveDocument.Path is ToLower but GitHub can't open lower path.
                // fix proper-casing | http://stackoverflow.com/questions/325931/getting-actual-file-name-with-proper-casing-on-windows-with-net
                path = GetExactPathName(DTE.ActiveDocument.Path + DTE.ActiveDocument.Name);
            }
            return path;
        }

        static string GetExactPathName(string pathName)
        {
            if (!(File.Exists(pathName) || Directory.Exists(pathName)))
                return pathName;

            var di = new DirectoryInfo(pathName);

            if (di.Parent != null)
            {
                return Path.Combine(
                    GetExactPathName(di.Parent.FullName),
                    di.Parent.GetFileSystemInfos(di.Name)[0].Name);
            }
            else
            {
                return di.Name.ToUpper();
            }
        }
    }
}
