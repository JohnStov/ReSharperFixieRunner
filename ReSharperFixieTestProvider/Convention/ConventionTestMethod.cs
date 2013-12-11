using System;
using System.Reflection;

namespace FixiePlugin.Convention
{
    [Serializable]
    public class ConventionTestMethod
    {
        public ConventionTestMethod(MethodInfo methodInfo)
        {
            MethodName = methodInfo.Name;
            ReturnType = methodInfo.ReturnType.FullName;
        }

        public string MethodName { get; private set; }

        public string ReturnType { get; private set; }
    }
}