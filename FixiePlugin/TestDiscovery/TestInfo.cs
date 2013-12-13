using System;
using System.Collections.Generic;
using System.Linq;
using FixiePlugin.Convention;

namespace FixiePlugin.TestDiscovery
{
    [Serializable]
    public class TestInfo
    {
        private readonly List<ConventionTestClass> classes;
        
        public TestInfo(IEnumerable<ConventionTestClass> classes)
        {
            this.classes = new List<ConventionTestClass>(classes);
        }

        public bool IsTestClass(string className)
        {
            return classes.Any(c => c.TypeName == className);
        }

        public bool IsTestMethod(string className, string methodName)
        {
            var @class = classes.FirstOrDefault(c => c.TypeName == className);

            if (@class == null)
                return false;

            return @class.IsTestMethod(methodName);
        }

        public bool IsParameterizedTestMethod(string className, string methodName)
        {
            var @class = classes.FirstOrDefault(c => c.TypeName == className);

            if (@class == null)
                return false;

            return @class.IsParameterizedTestMethod(methodName);
        }
    }
}