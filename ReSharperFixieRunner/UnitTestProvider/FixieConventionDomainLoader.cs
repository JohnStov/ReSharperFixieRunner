using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Fixie.Conventions;

using NuGet;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [Serializable]
    public class FixieConventionDomainLoader
    {
        private readonly string assemblyPath;

        private List<FixieConventionTestClass> classes = new List<FixieConventionTestClass>();

        public FixieConventionDomainLoader(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        public IEnumerable<FixieConventionTestClass> Classes { get { return classes; } }

        public void LoadTestClasses()
        {
            var appDomain = AppDomain.CurrentDomain;

            var testAssembly = Assembly.LoadFrom(assemblyPath);
            var conventionAssembly = testAssembly;
            var conventionTypes = Enumerable.ToArray<Type>(testAssembly.GetExportedTypes().Where(IsConvention));
            if (conventionTypes.IsEmpty())
            {
                try
                {
                    var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll");
                    conventionAssembly = Assembly.LoadFrom(fixieAssemblyPath);
                    conventionTypes = conventionAssembly.GetExportedTypes().Where(t => t.FullName == "Fixie.Conventions.DefaultConvention").ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot load Fixie assembly");
                }
            }

            foreach (var conventionType in conventionTypes)
            {
                var convention = appDomain.CreateInstanceAndUnwrap(conventionAssembly.FullName, conventionType.FullName) as Convention;

                IEnumerable<Type> types = convention.Classes.Filter(testAssembly.GetExportedTypes());
                foreach (var type in types)
                {
                    var classInfo = new FixieConventionTestClass(type);
                    classes.Add(classInfo);
                    var filteredMethods = convention.Methods.Filter(type);
                    foreach (MethodInfo method in filteredMethods)
                        classInfo.AddTestMethod(method);
                }
            }

            // remove duplicates
            classes = classes.Distinct(new FixieConventionTestClassComparer()).ToList();
        }

        private static bool IsConvention(Type type)
        {
            if (type.FullName == "System.Object")
                return false;

            if (type.FullName == "Fixie.Conventions.Convention")
                return true;

            return IsConvention(type.BaseType);
        }
    }
}