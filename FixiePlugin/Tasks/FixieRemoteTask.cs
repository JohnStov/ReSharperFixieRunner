using System.Xml;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public abstract class FixieRemoteTask : RemoteTask
    {
        protected FixieRemoteTask(XmlElement element)
            : base(element)
        {
        }

        protected FixieRemoteTask(string runnerId)
            : base(runnerId)
        {
        }

        public void CloseTask(TaskResult result, string message)
        {
            TaskResult = result;
            Message = message;
        }

        public TaskResult TaskResult { get; private set; }

        public string Message { get; private set; }
    }
}
