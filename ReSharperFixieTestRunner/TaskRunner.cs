using System;
using System.IO;
using System.Reflection;
using System.Xml;

using JetBrains.ProjectModel.Resources;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class TaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Fixie";

        private IRemoteRunner remoteRunner;

        public TaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var originalDirectory = Directory.GetCurrentDirectory();
            var pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(pluginDirectory);

            using (var appDomain = new AppDomainWrapper(pluginDirectory))
            {
                RunNode(appDomain, node);
            }

            Directory.SetCurrentDirectory(originalDirectory);
        }

        private void RunNode(AppDomainWrapper appDomain, TaskExecutionNode node)
        {
            Server.TaskStarting(node.RemoteTask);

            RunTask(appDomain, node.RemoteTask);

            foreach (var child in node.Children)
                RunNode(appDomain, child);

            Server.TaskFinished(node.RemoteTask, string.Empty, TaskResult.Success);
        }

        private void RunTask(AppDomainWrapper appDomain, RemoteTask remoteTask)
        {

            if (remoteTask is FixieTestAssemblyTask)
                RunAssemblyTask(appDomain, remoteTask as FixieTestAssemblyTask);
            if (remoteTask is FixieTestMethodTask)
                RunMethodTask(appDomain, remoteTask as FixieTestMethodTask);
        }

        private void RunAssemblyTask(AppDomainWrapper appDomain, FixieTestAssemblyTask fixieTestAssemblyTask)
        {
            remoteRunner = appDomain.CreateObject<IRemoteRunner>(
                AssemblyName.GetAssemblyName("FixieRemoteRunner.dll").FullName,
                "FixieRemoteRunner.RemoteRunner",
                new object[] {fixieTestAssemblyTask.AssemblyLocation});
        }

        private void RunMethodTask(AppDomainWrapper appDomain, FixieTestMethodTask fixieTestMethodTask)
        {
            if (remoteRunner != null)
            {
                var xdoc = new XmlDocument();
                var taskElement = xdoc.CreateElement("task");
                fixieTestMethodTask.SaveXml(taskElement);
                var taskInfo = taskElement.OuterXml;
                remoteRunner.RunTest(taskInfo);
            }
        }
    }

    public interface IRemoteRunner
    {
        void RunTest(string remoteTask);
    }
}
