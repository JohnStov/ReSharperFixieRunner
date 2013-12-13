using System;
using System.Xml;
using FixiePlugin.TestRun;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public class TestMethodTask : FixieRemoteTask, IEquatable<TestMethodTask>
    {
        public TestMethodTask(XmlElement element)
            : base(element)
        {
            AssemblyLocation = GetXmlAttribute(element, AttributeNames.AssemblyLocation);
            TypeName = GetXmlAttribute(element, AttributeNames.TypeName);
            MethodName = GetXmlAttribute(element, AttributeNames.MethodName);
            IsParameterized = bool.Parse(GetXmlAttribute(element, AttributeNames.IsParameterized));
        }
        
        public TestMethodTask(string assemblyLocation, string classTypeName, string methodName, bool isParameterized)
            : base((string) TaskRunner.RunnerId)
        {
            AssemblyLocation = assemblyLocation;
            TypeName = classTypeName;
            MethodName = methodName;
            IsParameterized = isParameterized;
        }

        public string AssemblyLocation { get; private set; }
        public string TypeName { get; private set; }
        public string MethodName { get; private set; }
        public bool IsParameterized { get; private set; }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, AttributeNames.AssemblyLocation, AssemblyLocation);
            SetXmlAttribute(element, AttributeNames.TypeName, TypeName);
            SetXmlAttribute(element, AttributeNames.MethodName, MethodName);
            SetXmlAttribute(element, AttributeNames.IsParameterized, IsParameterized.ToString());
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as TestMethodTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestMethodTask);
        }

        public bool Equals(TestMethodTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(AssemblyLocation, other.AssemblyLocation) &&
                   Equals(MethodName, other.MethodName);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Don't include base.GetHashCode, as RemoteTask.GetHashCode includes RemoteTask.Id
                // in the calculation, and this is a new guid generated for each new instance.
                // This would mean two instances that return true from Equals (i.e. value objects)
                // would have different hash codes
                int result = (TypeName != null ? TypeName.GetHashCode() : 0);
                result = (result * 397) ^ (MethodName != null ? MethodName.GetHashCode() : 0);
                result = (result * 397) ^ (AssemblyLocation != null ? AssemblyLocation.GetHashCode() : 0);
                return result;
            }
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override string ToString()
        {
            return string.Format("TestMethodTask<{0}>({1}.{2})", Id, TypeName, MethodName);
        }
    }
}