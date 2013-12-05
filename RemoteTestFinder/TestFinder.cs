using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using ReSharperFixieTestProvider;

namespace RemoteTestFinder
{
    [Serializable]
    public class TestFinder
    {
        public FixieConventionInfo FindTests(string testAssemblyPath)
        {
            var appDomain = AppDomain.CurrentDomain;

            var testAssembly = Assembly.LoadFrom(testAssemblyPath);
            var conventionAssembly = testAssembly;
            var conventionTypes = testAssembly.GetExportedTypes().Where(IsConvention).ToArray();
            if (!conventionTypes.Any())
            {
                var fixieAssemblyPath = Path.Combine(Directory.GetCurrentDirectory(), "Fixie.dll");
                conventionAssembly = Assembly.LoadFrom(fixieAssemblyPath);
                conventionTypes = conventionAssembly.GetExportedTypes().Where(t => t.FullName == "Fixie.Conventions.DefaultConvention").ToArray();
            }

            var testClasses = new List<FixieConventionTestClass>();
            foreach (var conventionType in conventionTypes)
            {
                var convention = (dynamic)appDomain.CreateInstanceAndUnwrap(conventionAssembly.FullName, conventionType.FullName);
                var classes = convention.Classes;
                if (classes != null)
                {
                    IEnumerable<Type> types = classes.Filter(testAssembly.GetExportedTypes());
                    foreach (var type in types)
                    {
                        var classInfo = new FixieConventionTestClass(type);

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
            return new FixieConventionInfo(testClasses.Distinct());
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
