using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FixiePlugin.Convention
{
    [Serializable]
    public class ConventionTestClass : IEqualityComparer<ConventionTestClass>
    {
        private readonly List<ConventionTestMethod> testMethods = new List<ConventionTestMethod>();

        public ConventionTestClass(Type type)
        {
            TypeName = type.FullName;
        }

        public string TypeName { get; private set; }

        public void AddTestMethod(MethodInfo methodInfo)
        {
            testMethods.Add(new ConventionTestMethod(methodInfo));
        }

        public bool IsTestMethod(string methodName)
        {
            return testMethods.Any(m => m.MethodName == methodName);
        }

        public bool Equals(ConventionTestClass x, ConventionTestClass y)
        {
            return x.TypeName == y.TypeName;
        }

        public int GetHashCode(ConventionTestClass obj)
        {
            return obj.GetHashCode();
        }

        public bool HasTestMethods()
        {
            return testMethods.Any();
        }

        public bool IsParameterizedTestMethod(string methodName)
        {
            var method = testMethods.FirstOrDefault(m => m.MethodName == methodName);
            if (method == null)
                return false;

            return method.IsParameterized;
        }
    }
}