using System;
using System.Reflection;

namespace ReSharperFixieTestProvider
{
    [Serializable]
    public class FixieConventionTestMethod
    {
        public FixieConventionTestMethod(MethodInfo methodInfo)
        {
            MethodName = methodInfo.Name;
            ReturnType = methodInfo.ReturnType.FullName;
        }

        public string MethodName { get; private set; }

        public string ReturnType { get; private set; }
    }
}