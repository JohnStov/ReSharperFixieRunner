using System;

namespace FixiePlugin.TestRun
{
    public class TestSetup : MarshalByRefObject
    {
        public TestSetup(string assemblyLocation, string typeName, string methodName)
        {
            AssemblyLocation = assemblyLocation;
            TypeName = typeName;
            MethodName = methodName;
        }

        public string AssemblyLocation { get; private set; }
        public string TypeName { get; private set; }
        public string MethodName { get; private set; }
    }
}