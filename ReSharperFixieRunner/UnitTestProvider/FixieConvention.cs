using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieConvention : MarshalByRefObject
    {
        private Assembly testAssembly;
        private dynamic convention;

        public FixieConvention(string path)
        {
            LoadConvention(path);
        }

        public Type[] GetTestClasses()
        {
            return convention.Classes.Filter(testAssembly.GetExportedTypes());
        }

        public MethodInfo[] GetTestMethods(Type type)
        {
            return convention.Methods.Filter(type);
        }

        public void LoadConvention(string assemblyPath)
        {
            testAssembly = Assembly.LoadFrom(assemblyPath);
            var conventionType = testAssembly.GetExportedTypes().FirstOrDefault(t => t.IsAssignableFrom(Type.GetType("Fixie.Conventions.Convention")));
            if (conventionType == null)
            {
                Assembly fixieAssembly = null;
                try
                {
                    var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(assemblyPath), "Fixie.dll");
                    fixieAssembly = Assembly.LoadFrom(fixieAssemblyPath);
                }
                catch(Exception ex)
                { }

                if (fixieAssembly != null)
                {
                    var exportedTypes = fixieAssembly.GetExportedTypes();
                    conventionType = exportedTypes.FirstOrDefault(t => t.FullName == "Fixie.Conventions.DefaultConvention");
                }
            }

            if (conventionType == null)
               throw new Exception("Cannot find Fixie convention");

            convention = Activator.CreateInstance(conventionType);
        }
    }
}