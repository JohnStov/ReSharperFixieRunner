using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReSharperFixieTestProvider.UnitTestProvider
{
    [Serializable]
    public class FixieConventionDomainLoader
    {
        private Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();

        public void LoadTestClasses()
        {
            var appDomain = AppDomain.CurrentDomain;
            var testAssemblyPath = (string)appDomain.GetData("TestAssemblyPath");
            var assemblyDirectory = Path.GetDirectoryName(testAssemblyPath);

            foreach (var assembly in Directory.EnumerateFiles(assemblyDirectory, "*.dll").Select(Assembly.LoadFrom))
                assemblies.Add(assembly.FullName, assembly);

            appDomain.AssemblyResolve += AssemblyResolve;
            
            try
            {
                var testAssembly = Assembly.LoadFrom(testAssemblyPath);
                var conventionAssembly = testAssembly;
                var conventionTypes = testAssembly.GetExportedTypes().Where(IsConvention).ToArray();
                if (!conventionTypes.Any())
                {
                    var fixieAssemblyPath = Path.Combine(assemblyDirectory, "Fixie.dll");
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
                            testClasses.Add(classInfo);

                            var methods = convention.Methods;
                            if (methods != null)
                            {
                                var filteredMethods = methods.Filter(type);
                                foreach (MethodInfo method in filteredMethods)
                                    classInfo.AddTestMethod(method);
                            }
                       }
                    }
                }

                // remove duplicates
                appDomain.SetData("TestClasses", testClasses.Distinct().ToArray());

                appDomain.AssemblyResolve -= AssemblyResolve;
            }
            catch (Exception ex)
            {
                appDomain.SetData("Exception", ex);
            }
        }

        private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assembly = assemblies[args.Name];
            return assembly;
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