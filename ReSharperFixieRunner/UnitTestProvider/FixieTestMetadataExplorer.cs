using System.Collections.Generic;
using System.Linq;
using System.Threading;

using JetBrains.Application;
using JetBrains.Decompiler.Render.CSharp;
using JetBrains.Interop.WinApi;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
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
            IMetadataAssembly assembly,
            UnitTestElementConsumer consumer,
            ManualResetEvent exitEvent)
        {
            using (ReadLockCookie.Create())
            {
                foreach (var metadataTypeInfo in GetExportedTypes(assembly.GetTypes()))
                    ExploreType(project, assembly, consumer, metadataTypeInfo);
            }
        }

        public IUnitTestProvider Provider { get { return provider; } }

        private static IEnumerable<IMetadataTypeInfo> GetExportedTypes(IEnumerable<IMetadataTypeInfo> types)
        {
            foreach (var type in (types ?? Enumerable.Empty<IMetadataTypeInfo>()).Where(IsPublic))
            {
                foreach (var nestedType in GetExportedTypes(type.GetNestedTypes()))
                {
                    yield return nestedType;
                }

                yield return type;
            }
        }

        private static bool IsPublic(IMetadataTypeInfo type)
        {
            return (type.IsNested && type.IsNestedPublic) || type.IsPublic;
        }

        private void ExploreType(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo)
        {
            if (IsTestClass(metadataTypeInfo))
                ExploreTestClass(project, assembly, consumer, metadataTypeInfo, metadataTypeInfo.FullyQualifiedName);
        }

        private void ExploreTestClass(IProject project, IMetadataAssembly assembly, UnitTestElementConsumer consumer, IMetadataTypeInfo metadataTypeInfo, string fullyQualifiedName)
        {
        }

        private bool IsTestClass(IMetadataTypeInfo metadataTypeInfo)
        {
            return metadataTypeInfo != null
                && metadataTypeInfo.IsClass
                && metadataTypeInfo.GetAccessRights() == CSharpAccessRights.Public
                // IL marks static class as sealed && abstract, so abstract check will find static classes too
                && !metadataTypeInfo.IsAbstract
                && metadataTypeInfo.GetMethods().Any(method => method.IsPublic && method.Parameters.Any())
                && metadataTypeInfo.Name.EndsWith("Tests");
        }

    }
}