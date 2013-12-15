using System.Xml;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.Tasks
{
    public abstract class FixieRemoteTask : RemoteTask
    {
        protected FixieRemoteTask(XmlElement element)
            : base(element)
        {
            Initialize();
        }

        protected FixieRemoteTask(string runnerId)
            : base(runnerId)
        {
            Initialize();
        }

        private void Initialize()
        {
            TaskResult = TaskResult.Inconclusive;
            Message = string.Empty;
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
