using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReSharperFixieTestProvider
{
    [Serializable]
    public class FixieConventionTestClass : IEqualityComparer<FixieConventionTestClass>
    {
        private readonly List<FixieConventionTestMethod> testMethods = new List<FixieConventionTestMethod>();

        public FixieConventionTestClass(Type type)
        {
            TypeName = type.FullName;
        }

        public string TypeName { get; private set; }

        public void AddTestMethod(MethodInfo methodInfo)
        {
            testMethods.Add(new FixieConventionTestMethod(methodInfo));
        }

        public bool IsTestMethod(string methodName)
        {
            return testMethods.Any(m => m.MethodName == methodName);
        }

        public bool Equals(FixieConventionTestClass x, FixieConventionTestClass y)
        {
            return x.TypeName == y.TypeName;
        }

        public int GetHashCode(FixieConventionTestClass obj)
        {
            return obj.GetHashCode();
        }

        public bool HasTestMethods()
        {
            return testMethods.Any();
        }
    }
}