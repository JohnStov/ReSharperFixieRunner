using System;
using System.Linq;
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
            IsParameterized = methodInfo.GetParameters().Any();
        }

        public string MethodName { get; private set; }

        public string ReturnType { get; private set; }

        public bool IsParameterized { get; private set; }
    }
}