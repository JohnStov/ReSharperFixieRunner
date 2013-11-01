using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReSharperFixieRunner.UnitTestProvider
{
    internal class FixieConventionTestClass
    {
        private readonly List<MethodInfo> testMethods = new List<MethodInfo>();

        public FixieConventionTestClass(Type type)
        {
            Type = type;
        }

        public Type Type { get; private set; }

        public void AddTestMethod(MethodInfo methodInfo)
        {
            testMethods.Add(methodInfo);
        }

        public bool IsTestMethod(string name)
        {
            return testMethods.Any(m => m.Name == name);
        }
    }
}