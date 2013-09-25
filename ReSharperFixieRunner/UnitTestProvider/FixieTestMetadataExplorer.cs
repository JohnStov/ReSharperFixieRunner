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
            //if (project.GetModuleReferences().All(module => module.Name != "Fixie"))
            //    return;

            var testAssembly = Assembly.LoadFile(assembly.Location.FullPath);

            var conventionType = testAssembly.GetExportedTypes().FirstOrDefault(t => t.IsAssignableFrom(Type.GetType("Fixie.Conventions.Convention")));
            if (conventionType == null)
            {
                Assembly fixieAssembly = null;
                try { fixieAssembly = Assembly.LoadFile(Path.Combine(assembly.Location.Directory.FullPath, "Fixie.dll")); }
                catch {}

                if (fixieAssembly != null)
                    conventionType =
                        fixieAssembly.GetExportedTypes()
                            .FirstOrDefault(t => t == Type.GetType("Fixie.Conventions.DefaultConvention"));
            }

            Convention convention = null;
            if (conventionType != null)
                convention = Activator.CreateInstance(conventionType) as Convention;

            if (convention == null)
                return;

            var testClasses = convention.Classes.Filter(testAssembly.GetExportedTypes());
            
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
                && metadataTypeInfo.Name.EndsWith("Tests");
        }

    }
}