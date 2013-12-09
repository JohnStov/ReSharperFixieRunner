using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

using Fixie;

using ReSharperFixieTestRunner;

namespace FixieRemoteRunner
{
    public class RemoteRunner : MarshalByRefObject, IRemoteRunner
    {
        private string assemblyPath;

        public RemoteRunner(string assemblyPath)
        {
            this.assemblyPath = assemblyPath;
        }

        public void RunTest(string remoteTask)
        {
            var doc = XDocument.Parse(remoteTask);
            var assemblyName = doc.Root.Attribute(AttributeNames.AssemblyLocation).Value;
            var typeName = doc.Root.Attribute(AttributeNames.TypeName).Value;
            var methodName = doc.Root.Attribute(AttributeNames.MethodName).Value;
            
            var domainName = AppDomain.CurrentDomain.FriendlyName;

            var listener = new ReSharperListener();
            var runner = new Runner(listener);

            Directory.SetCurrentDirectory(Path.GetDirectoryName(assemblyPath));
            var assembly = Assembly.LoadFile(assemblyPath);
            var type = assembly.GetType(typeName);
            var method = type.GetMethod(methodName);
            runner.RunMethod(assembly, method);
        }
    }

    public class ReSharperListener : Listener
    {
        public void AssemblyStarted(Assembly assembly)
        {
        }

        public void CasePassed(PassResult result)
        {
        }

        public void CaseFailed(FailResult result)
        {
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
        }
    }
}
