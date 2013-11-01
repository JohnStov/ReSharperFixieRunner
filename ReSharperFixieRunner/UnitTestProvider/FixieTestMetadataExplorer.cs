using System;
using System.IO;
using System.Reflection;
using System.Threading;
using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperFixieRunner.UnitTestProvider
{
    //[MetadataUnitTestExplorer]
    public class FixieTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly FixieTestProvider provider;
        private readonly UnitTestElementFactory unitTestElementFactory;

        public FixieTestMetadataExplorer(
            FixieTestProvider provider,
            UnitTestElementFactory unitTestElementFactory,
            UnitTestingAssemblyLoader assemblyLoader)
        {
            this.provider = provider;
            this.unitTestElementFactory = unitTestElementFactory;
        }


        public void ExploreAssembly(
            IProject project,
            IMetadataAssembly metadataAssembly,
            UnitTestElementConsumer consumer,
            ManualResetEvent exitEvent)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyLoadPath = Path.GetDirectoryName(assembly.Location);
            var assemblyName = assembly.FullName;
            var appDomain = AppDomain.CreateDomain("Fixie Domain", null, assemblyLoadPath, null, true);

            try
            {
                var constructorArgs = new object[] {metadataAssembly.Location.FullPath};
                var convention = (FixieConvention)appDomain.CreateInstanceAndUnwrap(assemblyName, "ReSharperFixieRunner.UnitTestProvider.FixieConvention", false, BindingFlags.ExactBinding, null, constructorArgs, null, null);
                var classes = convention.GetTestClasses();

                // if we can't find a convention, we can't run any tests
                if (convention != null)
                {
                    using (ReadLockCookie.Create())
                    {
                        foreach (var type in classes)
                            ExploreTestClass(project, metadataAssembly, consumer, type, convention);
                    }
                }

            }
            finally
            {
                AppDomain.Unload(appDomain);
            }

        }

        private IMetadataTypeInfo GetMetadataTypeInfo(Type type, IMetadataAssembly metadataAssembly)
        {
            return metadataAssembly.GetTypeInfoFromQualifiedName(type.FullName, false);
        }

        public IUnitTestProvider Provider { get { return provider; } }


        private void ExploreTestClass(IProject project, IMetadataAssembly metadataAssembly, UnitTestElementConsumer consumer, Type type, FixieConvention convention)
        {
            var metadataTypeInfo = GetMetadataTypeInfo(type, metadataAssembly);

            var classUnitTestElement = unitTestElementFactory.GetOrCreateTestClass(project, new ClrTypeName(metadataTypeInfo.FullyQualifiedName), metadataAssembly.Location.FullPath);
            consumer(classUnitTestElement);

            var testMethods = convention.GetTestMethods(type);
            foreach (var method in testMethods)
            {
                var methodUnitTestElement = unitTestElementFactory.GetOrCreateTestMethod(
                    project,
                    new ClrTypeName(metadataTypeInfo.FullyQualifiedName),
                    method.Name,
                    metadataAssembly.Location.FullPath);
                consumer(methodUnitTestElement);
            }

        }
    }
}