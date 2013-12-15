using System;
using System.Xml;
using FixiePlugin.TestRun;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public class ParameterizedTestMethodTask : FixieRemoteTask, IEquatable<ParameterizedTestMethodTask>
    {
        public ParameterizedTestMethodTask(XmlElement element)
            : base(element)
        {
            Name = element.GetAttribute(AttributeNames.MethodName);
        }

        public ParameterizedTestMethodTask(string name)
            : base(TaskRunner.RunnerId)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, AttributeNames.MethodName, Name);
        }

        public override bool Equals(RemoteTask remoteTask)
        {
            return Equals(remoteTask as ParameterizedTestMethodTask);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ParameterizedTestMethodTask);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public bool Equals(ParameterizedTestMethodTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Name, other.Name);
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override string ToString()
        {
            return string.Format("ParameterizedTestMethodTask <{0}>.{1})", Id, Name);
        }
    }
}