using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [Serializable]
    public class FixieConventionTestClass
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
    }
}