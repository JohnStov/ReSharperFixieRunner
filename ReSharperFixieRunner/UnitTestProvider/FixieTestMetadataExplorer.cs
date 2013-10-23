using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using Fixie.Conventions;

using JetBrains.Application;
using JetBrains.Decompiler.Render.CSharp;
using JetBrains.Interop.WinApi;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [MetadataUnitTestExplorer]
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
            var assembly = Assembly.LoadFile(metadataAssembly.Location.FullPath);
            var convention = FixieConvention.LoadConvention(assembly);
            // if we can't find a convention, we can't run any tests
            if (convention == null)
                return;

            using (ReadLockCookie.Create())
            {
                foreach (var type in convention.Classes.Filter(assembly.GetExportedTypes()))
                {
                    ExploreTestClass(project, metadataAssembly, consumer, type);
                }
            }
        }

        private IMetadataTypeInfo GetMetadataTypeInfo(Type type, IMetadataAssembly metadataAssembly)
        {
            return metadataAssembly.GetTypeInfoFromQualifiedName(type.FullName, false);
        }

        public IUnitTestProvider Provider { get { return provider; } }


        private void ExploreTestClass(IProject project, IMetadataAssembly metadataAssembly, UnitTestElementConsumer consumer, Type type)
        {
            var metadataTypeInfo = GetMetadataTypeInfo(type, metadataAssembly);

            var classUnitTestElement = unitTestElementFactory.GetOrCreateTestClass(project, new ClrTypeName(metadataTypeInfo.FullyQualifiedName), metadataAssembly.Location.FullPath);
            consumer(classUnitTestElement);
        }
    }
}