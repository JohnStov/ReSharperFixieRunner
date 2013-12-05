using System;
using System.Collections.Generic;
using System.Linq;

namespace ReSharperFixieTestProvider
{
    [Serializable]
    public class FixieConventionInfo
    {
        private readonly List<FixieConventionTestClass> classes;
        
        public FixieConventionInfo(IEnumerable<FixieConventionTestClass> classes)
        {
            this.classes = new List<FixieConventionTestClass>(classes);
        }

        public bool IsTestClass(string className)
        {
            return classes.Any(c => c.TypeName == className);
        }

        public bool IsTestMethod(string className, string methodName)
        {
            var @class = classes.FirstOrDefault(c => IsTestClass(className));

            if (@class == null)
                return false;

            return @class.IsTestMethod(methodName);
        }
    }
}