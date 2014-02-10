using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

namespace FixiePlugin.TestDiscovery
{
    [SolutionComponent]
    public class TestIdentifier
    {
        private readonly Dictionary<string, TestInfo> conventionCache = new Dictionary<string, TestInfo>();
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

        public bool IsValidTestMethod(IProject project, IMetadataMethod metadataMethod)
        {
            if (project == null || metadataMethod == null)
                return false;

            return IsValidTestMethod(project, metadataMethod.DeclaringType.FullyQualifiedName, metadataMethod.Name);
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

        public bool IsParameterizedMethod(IProject project, IClass testClass, IMethod testMethod)
        {
            if (project == null || testClass == null || testMethod == null)
                return false;

            return IsParameterizedMethod(project, testClass.GetClrName().FullName, testMethod.ShortName);
        }

        public bool IsParameterizedMethod(IProject project, string className, string methodName)
        {
            var conventionInfo = GetConventionInfo(project.GetOutputFilePath().FullPath);
            if (conventionInfo == null)
                return false;

            return conventionInfo.IsParameterizedTestMethod(className, methodName);
        }

        private TestInfo GetConventionInfo(string assemblyPath)
        {
            if (!conventionCache.ContainsKey(assemblyPath))
            {
                var result = LocalTestFinder.GetConventionInfo(assemblyPath);
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
