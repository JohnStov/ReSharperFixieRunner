using System;
using System.Xml;
using FixiePlugin.TestRun;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public class TestAssemblyTask : RemoteTask, IEquatable<TestAssemblyTask>
    {
        public string AssemblyLocation { get; private set; }

        public TestAssemblyTask(XmlElement element)
            : base(element)
        {
            AssemblyLocation = GetXmlAttribute(element, AttributeNames.AssemblyLocation);
        }

        public TestAssemblyTask(string assemblyLocation)
            :base((string) TaskRunner.RunnerId)
        {
            AssemblyLocation = assemblyLocation;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, AttributeNames.AssemblyLocation, AssemblyLocation);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as TestAssemblyTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestAssemblyTask);
        }

        public bool Equals(TestAssemblyTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(AssemblyLocation, other.AssemblyLocation);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Don't include base.GetHashCode, as RemoteTask.GetHashCode includes RemoteTask.Id
                // in the calculation, and this is a new guid generated for each new instance.
                // This would mean two instances that return true from Equals (i.e. value objects)
                // would have different hash codes
                return AssemblyLocation != null ? AssemblyLocation.GetHashCode() : 0;
            }
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }
    }
}