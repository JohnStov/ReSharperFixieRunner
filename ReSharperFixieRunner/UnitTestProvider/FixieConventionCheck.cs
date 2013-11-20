using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace ReSharperFixieTestProvider.UnitTestProvider
{
    [SolutionComponent]
    public class FixieConventionCheck
    {
        private readonly Dictionary<string, FixieConventionInfo> conventionCache = new Dictionary<string, FixieConventionInfo>();
        public readonly Dictionary<string, FileSystemWatcher> Watchers = new Dictionary<string, FileSystemWatcher>();

        public bool IsValidTestClass(IProject project, IMetadataTypeInfo typeInfo)
        {
            if (project == null || typeInfo == null)
                return false;

            return IsValidTestClass(project, typeInfo.FullyQualifiedName);
        }

        public bool IsValidTestClass(IProject project, IClass testClass)
        {
            if (project == null || testClass == null)
                return false;

            return IsValidTestClass(project, testClass.GetClrName().FullName);
        }


        private bool IsValidTestClass(IProject project, string className)
        {
            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsTestClass(className);
        }

        public bool IsValidTestMethod(IProject project, Type type, MethodInfo method)
        {
            if (project == null || type == null || method == null)
                return false;

            return IsValidTestMethod(project, type.FullName, method.Name);
        }

        public bool IsValidTestMethod(IProject project, IClass testClass, IMethod testMethod)
        {
            if (project == null || testClass == null || testMethod == null)
                return false;

            return IsValidTestMethod(project, testClass.GetClrName().FullName, testMethod.ShortName);
        }

        private bool IsValidTestMethod(IProject project, string className, string methodName)
        {
            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsTestMethod(className, methodName);
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
