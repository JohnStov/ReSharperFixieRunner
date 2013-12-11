using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using JetBrains.Metadata.Reader.API;
using JetBrains.Util;
using NSubstitute;

namespace FixiePlugin.Tests.TestDiscovery
{
    public class FixieTestMetadataExplorerTests
    {
        private void CreateExplorer()
        {
            var explorer = new TestMetadataExplorer(null, null, null, null);

            var assembly = Substitute.For<IMetadataAssembly>();
            assembly.Location.Returns(FileSystemPath.Parse(@"C:\Users\John\Documents\GitHub\ReSharperFixieRunner\FixieTestExample\FixieTestExample\bin\Debug\FixieTestExample.dll"));

            explorer.ExploreAssembly(null, assembly, null, null);
        }

        private void CanCreateAppDomain()
        {
            var assemblyPath = @"C:\Users\John\Documents\GitHub\ReSharperFixieRunner\FixieTestExample\FixieTestExample\bin\Debug\FixieTestExample.dll";
            var assemblyLoadPath = Path.GetDirectoryName(assemblyPath);
            var localAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = localAssemblyPath,
                PrivateBinPath = assemblyLoadPath,
                ShadowCopyFiles = "true",
            };

            var appDomain = AppDomain.CreateDomain("TestDomain", null, appDomainSetup);

            var thingy = new Thingy(assemblyPath);
            appDomain.DoCallBack(thingy.LoadInAppDomain);

            AppDomain.Unload(appDomain);

            foreach (var testClass in thingy.TestClasses)
                Debug.WriteLine(testClass.FullName);
            foreach (var testMethod in thingy.TestMethods)
                Debug.WriteLine(testMethod.Name);
        }
    }

    [Serializable]
    public class Thingy
    {
        private readonly string assemblyPath;
        private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        private readonly List<Type> testClasses = new List<Type>();
        private readonly List<MethodInfo> testMethods = new List<MethodInfo>();


        public Thingy(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        public IEnumerable<Type> TestClasses { get { return testClasses; } }
        public IEnumerable<MethodInfo> TestMethods { get { return testMethods; } }

        public void LoadInAppDomain()
        {
            var assemblyDir = Path.GetDirectoryName(assemblyPath);
            foreach (var assemblyName in Directory.EnumerateFiles(assemblyDir, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(assemblyName);
                assemblies.Add(assembly.FullName, assembly);
            }

            var domain = AppDomain.CurrentDomain;
            var domainName = domain.FriendlyName;
            domain.AssemblyLoad += AppDomainOnAssemblyLoad;
            domain.AssemblyResolve += DomainOnAssemblyResolve;

            var fixieDllPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll");
            Assembly.LoadFrom(fixieDllPath);

            var dllAssemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var dllAssembly = Assembly.Load(dllAssemblyName);
            var assemblyClasses = dllAssembly.GetExportedTypes();
            var conventionClasses = assemblyClasses.Where(t => t.BaseType.FullName == "Fixie.Conventions.Convention").ToArray();

            var convention = domain.CreateInstanceAndUnwrap(dllAssemblyName.FullName, conventionClasses[0].FullName) as dynamic;
            testClasses.AddRange(convention.Classes.Filter(assemblyClasses));

            foreach (Type testClass in testClasses)
                testMethods.AddRange(convention.Methods.Filter(testClass));
        }

        private Assembly DomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return assemblies[args.Name];
        }

        private void AppDomainOnAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            if (!assemblies.ContainsKey(args.LoadedAssembly.FullName))
                assemblies.Add(args.LoadedAssembly.FullName, args.LoadedAssembly);
        }
    }
}
