using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperFixieTestProvider.UnitTestProvider
{
    [MetadataUnitTestExplorer]
    public class FixieTestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly FixieTestProvider provider;
        private readonly FixieConventionCheck conventionCheck;
        private readonly UnitTestElementFactory unitTestElementFactory;

        public FixieTestMetadataExplorer(
            FixieTestProvider provider,
            FixieConventionCheck conventionCheck,
            UnitTestElementFactory unitTestElementFactory,
            UnitTestingAssemblyLoader assemblyLoader)
        {
            this.provider = provider;
            this.conventionCheck = conventionCheck;
            this.unitTestElementFactory = unitTestElementFactory;
        }


        public void ExploreAssembly(
            IProject project,
            IMetadataAssembly metadataAssembly,
            UnitTestElementConsumer consumer,
            ManualResetEvent exitEvent)
        {
            if (project.GetModuleReferences().All(module => module.Name != "Fixie"))
                return;

            using (ReadLockCookie.Create())
            {
                foreach (var metadataTypeInfo in GetExportedTypes(metadataAssembly.GetTypes()))
                    ExploreType(project, metadataAssembly, consumer, metadataTypeInfo);
            }
        }

        // ReSharper's IMetadataAssembly.GetExportedTypes always seems to return an empty list, so
        // let's roll our own. MSDN says that Assembly.GetExportTypes is looking for "The only types
        // visible outside an assembly are public types and public types nested within other public types."
        // TODO: It might be nice to randomise this list:
        // However, this returns items in alphabetical ordering. Assembly.GetExportedTypes returns back in
        // the order in which classes are compiled (so the order in which their files appear in the msbuild file!)
        // with dependencies appearing first. 
        // Stolen lock, stock and barrel from Matt Eliss's (twitter @citizenmatt) xunit runner 
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

        private IMetadataTypeInfo GetMetadataTypeInfo(Type type, IMetadataAssembly metadataAssembly)
        {
            return metadataAssembly.GetTypeInfoFromQualifiedName(type.FullName, false);
        }

        public IUnitTestProvider Provider { get { return provider; } }


        private void ExploreType(IProject project, IMetadataAssembly metadataAssembly, UnitTestElementConsumer consumer, IMetadataTypeInfo typeInfo)
        {
            if (conventionCheck.IsValidTestClass(project, typeInfo))
            {
                var classUnitTestElement = unitTestElementFactory.GetOrCreateTestClass(project, new ClrTypeName(typeInfo.FullyQualifiedName), metadataAssembly.Location.FullPath);
                consumer(classUnitTestElement);
            }
        }
    }
}