using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using FixiePlugin.Convention;

namespace FixiePlugin.TestDiscovery
{
    public class RemoteTestFinder : MarshalByRefObject
    {
        public TestInfo FindTests(string testAssemblyPath)
        {
            var testAssembly = Assembly.LoadFrom(testAssemblyPath);
            var conventionTypes = testAssembly.GetExportedTypes().Where(IsConvention).ToArray();

            if (!conventionTypes.Any())
            {
                var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(testAssemblyPath), "Fixie.dll");
                var fixieAssembly = Assembly.LoadFrom(fixieAssemblyPath);
                conventionTypes = fixieAssembly.GetExportedTypes().Where(t => t.FullName == "Fixie.Conventions.DefaultConvention").ToArray();
            }

            var testClasses = new List<ConventionTestClass>();
            foreach (var conventionType in conventionTypes)
            {
                var convention = (dynamic)Activator.CreateInstance(conventionType);

                var classes = convention.Classes;
                if (classes != null)
                {
                    IEnumerable<Type> types = classes.Filter(testAssembly.GetExportedTypes());
                    foreach (var type in types)
                    {
                        var classInfo = new ConventionTestClass(type);

                        var methods = convention.Methods;
                        if (methods != null)
                        {
                            var filteredMethods = methods.Filter(type);
                            foreach (MethodInfo method in filteredMethods)
                                classInfo.AddTestMethod(method);
                        }

                        if (classInfo.HasTestMethods())
                            testClasses.Add(classInfo);
                    }
                }
            }
            // remove duplicates
            return new TestInfo(testClasses.Distinct());
        }

        private static bool IsConvention(Type type)
        {
            if (type == null || type.FullName == "System.Object")
                return false;

            if (type.FullName == "Fixie.Conventions.Convention")
                return true;

            return IsConvention(type.BaseType);
        }
    }
}
