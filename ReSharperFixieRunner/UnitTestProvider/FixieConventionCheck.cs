using System.Collections.Generic;
using System.IO;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [SolutionComponent]
    public class FixieConventionCheck
    {
        private readonly Dictionary<string, FixieConventionInfo> conventionCache = new Dictionary<string, FixieConventionInfo>();

        public readonly Dictionary<string, FileSystemWatcher> Watchers = new Dictionary<string, FileSystemWatcher>();
        
        public bool IsValidTestClass(IProject project, IClass testClass)
        {
            if (project == null || testClass == null)
                return false;

            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsTestClass(testClass.GetClrName().FullName);
        }

        public bool IsValidTestMethod(IProject project, IClass testClass, IMethod testMethod)
        {
            if (project == null || testClass == null)
                return false;

            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsTestMethod(testClass.GetClrName().FullName, testMethod.ShortName);
        }

        private FixieConventionInfo GetConventionInfo(string assemblyPath)
        {
            if (!conventionCache.ContainsKey(assemblyPath))
            {
                var result = FixieConventionLoader.GetConventionInfo(assemblyPath);
                if (result == null)
                    return null;

                conventionCache.Add(assemblyPath, result);

                var assemblyFolder = Path.GetDirectoryName(assemblyPath);
                if (!Watchers.ContainsKey(assemblyFolder))
                {
                    var watcher = new FileSystemWatcher(assemblyFolder);
                    watcher.Changed += WatcherOnChanged;
                    watcher.Deleted += WatcherOnChanged;
                    watcher.EnableRaisingEvents = true;
                    Watchers.Add(assemblyFolder, watcher);
                }
            }
            return conventionCache[assemblyPath];
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (conventionCache.ContainsKey(fileSystemEventArgs.FullPath))
                conventionCache.Remove(fileSystemEventArgs.FullPath);
        }
    }
}
