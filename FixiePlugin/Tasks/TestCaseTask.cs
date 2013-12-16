using System;
using System.Text.RegularExpressions;
using System.Xml;
using FixiePlugin.TestRun;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public class TestCaseTask : FixieRemoteTask, IEquatable<TestCaseTask>
    {
        private static readonly Regex MethodRegex = new Regex(@"(?<type>.*)\.(?<method>.*)(?<args>\(.*\))");

        public TestCaseTask(XmlElement element)
            : base(element)
        {
            TypeName = GetXmlAttribute(element, AttributeNames.TypeName);
            MethodName = GetXmlAttribute(element, AttributeNames.MethodName);
            Parameters = GetXmlAttribute(element, AttributeNames.Parameters);
        }

        public TestCaseTask(string assemblyLocation, string name)
            : base(TaskRunner.RunnerId, assemblyLocation)
        {
            var matches = MethodRegex.Match(name);
            TypeName = matches.Groups["type"].Value;
            MethodName = matches.Groups["method"].Value;
            Parameters = matches.Groups["args"].Value;
        }

        public string TypeName { get; private set; }
        public string MethodName { get; private set; }
        public string Parameters { get; private set; }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, AttributeNames.AssemblyLocation, AssemblyLocation);
            SetXmlAttribute(element, AttributeNames.TypeName, TypeName);
            SetXmlAttribute(element, AttributeNames.MethodName, MethodName);
            SetXmlAttribute(element, AttributeNames.Parameters, Parameters);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as TestCaseTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TestCaseTask);
        }

        public bool Equals(TestCaseTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            // Don't include base.Equals, as RemoteTask.Equals includes RemoteTask.Id
            // in the calculation, and this is a new guid generated for each new instance
            // Using RemoteTask.Id in the Equals means collapsing the return values of
            // IUnitTestElement.GetTaskSequence into a tree will fail (as no assembly,
            // or class tasks will return true from Equals)
            return Equals(Parameters, other.Parameters) &&
                   Equals(MethodName, other.MethodName) &&
                   Equals(TypeName, other.MethodName) &&
                   Equals(AssemblyLocation, other.AssemblyLocation);
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
                result = (result * 397) ^ (Parameters != null ? Parameters.GetHashCode() : 0);
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
            return string.Format("TestCaseTask<{0}>({1}.{2}.{3})", Id, TypeName, MethodName, Parameters);
        }
    }
}