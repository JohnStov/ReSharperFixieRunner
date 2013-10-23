using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Fixie.Conventions;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieConvention
    {
        private readonly dynamic convention;

        private FixieConvention(object convention)
        {
            this.convention = convention;
        }

        public ClassFilter Classes
        {
            get
            {
                return convention.Classes;
            }

        }

        public static FixieConvention LoadConvention(Assembly assembly)
        {
            var conventionType = assembly.GetExportedTypes().FirstOrDefault(t => t.IsAssignableFrom(Type.GetType("Fixie.Conventions.Convention")));
            if (conventionType == null)
             {
                Assembly fixieAssembly = null;
                var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "Fixie.dll");
                try { fixieAssembly = Assembly.LoadFile(fixieAssemblyPath); }
                catch (Exception)
                { }

                if (fixieAssembly != null)
                {
                    var exportedTypes = fixieAssembly.GetExportedTypes();
                    conventionType = exportedTypes.FirstOrDefault(t => t.FullName == "Fixie.Conventions.DefaultConvention");
                }
            }

            if (conventionType == null)
                return null;

            var convention = Activator.CreateInstance(conventionType);
            return new FixieConvention(convention);
        }
    }
}