using System;
using System.Globalization;
using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class FixieTestMethodTask : RemoteTask, IEquatable<FixieTestMethodTask>
    {
        private readonly string assemblyLocation;
        private readonly string typeName;
        private readonly string methodName;
        private readonly bool explicitly;
        private readonly bool isDynamic;
        
        public FixieTestMethodTask(string assemblyLocation, string classTypeName, string methodName, bool explicitly, bool isDynamic)
            : base(FixieTaskRunner.RunnerId)
        {
            this.assemblyLocation = assemblyLocation;
            this.typeName = classTypeName;
            this.methodName = methodName;
            this.explicitly = explicitly;
            this.isDynamic = isDynamic;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, "AssemblyLocation", assemblyLocation);
            SetXmlAttribute(element, "TypeName", typeName);
            SetXmlAttribute(element, "MethodName", methodName);
            SetXmlAttribute(element, "Explicitly", explicitly.ToString(CultureInfo.InvariantCulture));
            SetXmlAttribute(element, "Dynamic", isDynamic.ToString(CultureInfo.InvariantCulture));
        }

        public override RuntimeEnvironment EnsureRuntimeEnvironment(RuntimeEnvironment runtimeEnvironment)
        {
            return base.EnsureRuntimeEnvironment(runtimeEnvironment);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as FixieTestMethodTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FixieTestMethodTask);
        }

        public bool Equals(FixieTestMethodTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(assemblyLocation, other.assemblyLocation) &&
                   Equals(methodName, other.methodName) &&
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
                int result = explicitly.GetHashCode();
                result = (result * 397) ^ (typeName != null ? typeName.GetHashCode() : 0);
                result = (result * 397) ^ (methodName != null ? methodName.GetHashCode() : 0);
                result = (result * 397) ^ (assemblyLocation != null ? assemblyLocation.GetHashCode() : 0);
                return result;
            }
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override string ToString()
        {
            return string.Format("FixieTestMethodTask<{0}>({1}.{2})", Id, typeName, methodName);
        }
    }
}