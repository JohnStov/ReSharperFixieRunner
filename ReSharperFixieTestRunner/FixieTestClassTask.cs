using System;
using System.Globalization;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class FixieTestClassTask : RemoteTask, IEquatable<FixieTestClassTask>
    {
        private readonly string assemblyLocation;
        private readonly string typeName;
        private readonly bool explicitly;

        public FixieTestClassTask(string assemblyLocation, string typeName, bool explicitly)
            : base(FixieTaskRunner.RunnerId)
        {
            this.assemblyLocation = assemblyLocation;
            this.typeName = typeName;
            this.explicitly = explicitly;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "AssemblyLocation", assemblyLocation);
            SetXmlAttribute(element, "TypeName", typeName);
            SetXmlAttribute(element, "Explicitly", explicitly.ToString(CultureInfo.InvariantCulture));
        }

        public override RuntimeEnvironment EnsureRuntimeEnvironment(RuntimeEnvironment runtimeEnvironment)
        {
            return base.EnsureRuntimeEnvironment(runtimeEnvironment);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as FixieTestClassTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FixieTestClassTask);
        }

        public bool Equals(FixieTestClassTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(assemblyLocation, other.assemblyLocation) &&
                   Equals(typeName, other.typeName) &&
                   explicitly == other.explicitly;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                // Don't include base.GetHashCode, as RemoteTask.GetHashCode includes RemoteTask.Id
                // in the calculation, and this is a new guid generated for each new instance.
                // This would mean two instances that return true from Equals (i.e. value objects)
                // would have different hash codes
                int result = (assemblyLocation != null ? assemblyLocation.GetHashCode() : 0);
                result = (result * 397) ^ (typeName != null ? typeName.GetHashCode() : 0);
                result = (result * 397) ^ explicitly.GetHashCode();
                return result;
            }
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }


        public override string ToString()
        {
            return string.Format("FixieTestClassTask<{0}>({1})", Id, typeName);
        }
    }
}