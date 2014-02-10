using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FixiePlugin.TestDiscovery;
using JetBrains.Application;
using JetBrains.Metadata.Reader.API;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin
{
    [MetadataUnitTestExplorer]
    public class TestMetadataExplorer : IUnitTestMetadataExplorer
    {
        private readonly TestProvider provider;
        private readonly TestIdentifier conventionCheck;
        private readonly UnitTestElementFactory unitTestElementFactory;

        public TestMetadataExplorer(
            TestProvider provider,
            TestIdentifier conventionCheck,
            UnitTestElementFactory unitTestElementFactory)
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
            using (ReadLockCookie.Create())
            {
                if (project.GetModuleReferences().All(module => module.Name != "Fixie"))
                    return;

                foreach (var metadataTypeInfo in GetExportedTypes(metadataAssembly.GetTypes()))
                {
                    bool isTestClass = ExploreType(project, metadataAssembly, consumer, metadataTypeInfo);
                    if (isTestClass)
                    {
                        foreach(var metadataMethod in metadataTypeInfo.GetMethods())
                            ExploreMethod(project, metadataAssembly, consumer, metadataMethod);
                    }
                }
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

        public IUnitTestProvider Provider { get { return provider; } }


        private bool ExploreType(IProject project, IMetadataAssembly metadataAssembly, UnitTestElementConsumer consumer, IMetadataTypeInfo typeInfo)
        {
            if (conventionCheck.IsValidTestClass(project, typeInfo))
            {
                var classUnitTestElement = unitTestElementFactory.GetOrCreateTestClass(project, new ClrTypeName(typeInfo.FullyQualifiedName), metadataAssembly.Location.FullPath);
                consumer(classUnitTestElement);
                return true;
            }

            return false;
        }

        private void ExploreMethod(IProject project, IMetadataAssembly metadataAssembly, UnitTestElementConsumer consumer, IMetadataMethod metadataMethod)
        {
            if (conventionCheck.IsValidTestMethod(project, metadataMethod))
            {
                bool isParameterised = conventionCheck.IsParameterizedMethod(
                    project,
                    metadataMethod.DeclaringType.FullyQualifiedName,
                    metadataMethod.Name);

                var methodUnitTestElement = unitTestElementFactory.GetOrCreateTestMethod(project, new ClrTypeName(metadataMethod.DeclaringType.FullyQualifiedName), metadataMethod.Name, metadataAssembly.Location.FullPath, isParameterised);
                consumer(methodUnitTestElement);
            }
        }
    }
}