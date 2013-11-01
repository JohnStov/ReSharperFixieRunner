using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fixie.Conventions;
using NuGet;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieConventionLoader : MarshalByRefObject
    {
        public IEnumerable<FixieConventionTestClass> LoadTestClasses(string assemblyPath)
        {
            var appDomain = AppDomain.CurrentDomain;
            var appDomainName = appDomain.FriendlyName;
            var assemblyName = AssemblyName.GetAssemblyName(assemblyPath);
            var testAssembly = appDomain.Load(assemblyName);
            var conventionTypes = testAssembly.GetExportedTypes().Where(t => t.IsAssignableFrom(Type.GetType("Fixie.Conventions.Convention"))).ToArray();
            if (conventionTypes.IsEmpty())
            {
                try
                {
                    var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll");
                    assemblyName = AssemblyName.GetAssemblyName(fixieAssemblyPath);
                    var fixieAssembly = appDomain.Load(assemblyName);
                    var exportedTypes = fixieAssembly.GetExportedTypes();
                    conventionTypes = exportedTypes.Where(t => t.FullName == "Fixie.Conventions.DefaultConvention").ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot load Fixie assembly");
                }
            }

            var classes = new List<FixieConventionTestClass>();
            
            foreach (var conventionType in conventionTypes)
            {
                var convention = appDomain.CreateInstanceAndUnwrap(assemblyName.FullName, conventionType.FullName) as Convention;
                IEnumerable<Type> types = convention.Classes.Filter(testAssembly.GetExportedTypes());
                foreach (var type in types)
                {
                    var classInfo = new FixieConventionTestClass(type);
                    classes.Add(classInfo);
                    var filteredMethods = convention.Methods.Filter(type);
                    foreach (MethodInfo method in filteredMethods)
                    {
                        classInfo.AddTestMethod(method);
                    }
                }
            }

            // remove duplicates
            return classes.Distinct(new FixieConventionTestClassComparer()).ToArray();
        }

        public static FixieConventionInfo GetConventionInfo(string assemblyPath)
        {
            IEnumerable<FixieConventionTestClass> classes;
            
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.FullName;
            var assemblyLoadPath = Path.GetDirectoryName(assembly.Location);

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = assemblyLoadPath,
                ShadowCopyFiles = "true",
            };

            var appDomain = AppDomain.CreateDomain("FixieConventionLoader", null, appDomainSetup);

            var className = MethodBase.GetCurrentMethod().DeclaringType.FullName;
            var loader = appDomain.CreateInstanceAndUnwrap(assemblyName, className) as FixieConventionLoader;

            classes = loader.LoadTestClasses(assemblyPath);

            AppDomain.Unload(appDomain);
            
            return new FixieConventionInfo(classes);
        }
    }
}
