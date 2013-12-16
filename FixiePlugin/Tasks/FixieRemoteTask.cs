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
            AssemblyLocation = GetXmlAttribute(element, AttributeNames.AssemblyLocation);
        }

        protected FixieRemoteTask(string runnerId, string assemblyLocation)
            : base(runnerId)
        {
            Initialize();
            AssemblyLocation = assemblyLocation;
        }

        private void Initialize()
        {
            TaskResult = TaskResult.Inconclusive;
            Message = string.Empty;
        }

        public override void SaveXml(XmlElement element)
        {
            base.SaveXml(element);
            SetXmlAttribute(element, AttributeNames.AssemblyLocation, AssemblyLocation);
        }

        public void CloseTask(TaskResult result, string message)
        {
            TaskResult = result;
            Message = message;
        }

        public TaskResult TaskResult { get; private set; }

        public string Message { get; private set; }

        public string AssemblyLocation { get; private set; }

    }
}
