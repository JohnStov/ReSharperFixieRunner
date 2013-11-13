using System;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieRunner.TestRunner
{
    public class FixieTestAssemblyTask : RemoteTask, IEquatable<FixieTestAssemblyTask>
    {
        private readonly string assemblyLocation;

        public FixieTestAssemblyTask(string assemblyLocation)
            :base(FixieTaskRunner.RunnerId)
        {
            this.assemblyLocation = assemblyLocation;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "AssemblyLocation", assemblyLocation);
        }

        public override RuntimeEnvironment EnsureRuntimeEnvironment(RuntimeEnvironment runtimeEnvironment)
        {
            return base.EnsureRuntimeEnvironment(runtimeEnvironment);
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
            return Equals(assemblyLocation, other.assemblyLocation);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Don't include base.GetHashCode, as RemoteTask.GetHashCode includes RemoteTask.Id
                // in the calculation, and this is a new guid generated for each new instance.
                // This would mean two instances that return true from Equals (i.e. value objects)
                // would have different hash codes
                return assemblyLocation != null ? assemblyLocation.GetHashCode() : 0;
            }
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }
    }
}