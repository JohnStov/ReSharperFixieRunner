using System;
using System.Xml;
using FixiePlugin.TestRun;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public class ParameterizedTestMethodTask : FixieRemoteTask, IEquatable<ParameterizedTestMethodTask>
    {
        public ParameterizedTestMethodTask (XmlElement element)
            : base(element)
        {
        }

        public ParameterizedTestMethodTask()
            : base(TaskRunner.RunnerId)
        {
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
            return 0;
        }

        public bool Equals(ParameterizedTestMethodTask other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other);
        }

        public override bool IsMeaningfulTask
        {
            get { return true; }
        }

        public override string ToString()
        {
            return string.Format("ParameterizedTestMethodTask <{0}>)", Id);
        }
    }
}