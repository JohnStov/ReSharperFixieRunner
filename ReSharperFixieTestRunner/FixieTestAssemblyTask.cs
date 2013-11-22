using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class FixieTestAssemblyTask : RemoteTask, IEquatable<FixieTestAssemblyTask>
    {
        public string AssemblyLocation { get; private set; }

        public FixieTestAssemblyTask(XmlElement element)
            : base(element)
        {
            AssemblyLocation = GetXmlAttribute(element, AttributeNames.AssemblyLocation);
        }

        public FixieTestAssemblyTask(string assemblyLocation)
            :base(TaskRunner.RunnerId)
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
            return Equals(remoteTask as FixieTestAssemblyTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FixieTestAssemblyTask);
        }

        public bool Equals(FixieTestAssemblyTask other)
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