using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using JetBrains.Util;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieConventionInfo
    {
        private List<FixieConventionTestClass> classes = new List<FixieConventionTestClass>();
        
        private FixieConventionInfo(string path)
        {
            LoadConventions(path);
        }

        private void LoadConventions(string assemblyPath)
        {
            var testAssembly = Assembly.LoadFrom(assemblyPath);
            var conventionTypes = testAssembly.GetExportedTypes().Where(t => t.IsAssignableFrom(Type.GetType("Fixie.Conventions.Convention"))).ToArray();
            if (conventionTypes.IsEmpty())
            {
                try
                {
                    var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll");
                    var fixieAssembly = Assembly.LoadFrom(fixieAssemblyPath);
                    var exportedTypes = fixieAssembly.GetExportedTypes();
                    conventionTypes = exportedTypes.Where(t => t.FullName == "Fixie.Conventions.DefaultConvention").ToArray();
                }
                catch (Exception ex)
                {
                    throw new Exception("Cannot load Fixie assembly");
                }
            }

            foreach (var conventionType in conventionTypes)
            {
                dynamic convention = Activator.CreateInstance(conventionType);
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
            classes = classes.Distinct(new FixieConventionTestClassComparer()).ToList();
        }

        public static FixieConventionInfo Create(string fullPath)
        {
            return File.Exists(fullPath) ? new FixieConventionInfo(fullPath) : null;
        }

        public bool IsTestClass(string className)
        {
            return classes.Any(c => c.Type.FullName == className);
        }

        public bool IsTestMethod(string className, string methodName)
        {
            var @class = classes.FirstOrDefault(c => IsTestClass(c.Type.FullName));

            if (@class == null)
                return false;

            return @class.IsTestMethod(methodName);
        }
    }
}