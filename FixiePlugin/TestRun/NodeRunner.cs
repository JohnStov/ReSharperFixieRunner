using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FixiePlugin.Tasks;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.Util;

namespace FixiePlugin.TestRun
{
    public class NodeRunner
    {
        private readonly IRemoteTaskServer server;
        private IRemoteRunner remoteRunner;

        public NodeRunner(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public void RunNode(TaskExecutionNode node)
        {
            var originalDirectory = Directory.GetCurrentDirectory();
            var pluginDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(pluginDirectory);

            using (var appDomain = new AppDomainWrapper(pluginDirectory))
            {
                RunNode(appDomain, node);
            }

            Directory.SetCurrentDirectory(originalDirectory);
            server.TaskStarting(node.RemoteTask);
        }

        private void RunNode(AppDomainWrapper appDomain, TaskExecutionNode node)
        {
            var success = RunTask(appDomain, node.RemoteTask);

            foreach (var child in node.Children)
                RunNode(appDomain, child);

            server.TaskFinished(node.RemoteTask, string.Empty, success ? TaskResult.Success : TaskResult.Error);
        }

        private bool RunTask(AppDomainWrapper appDomain, RemoteTask remoteTask)
        {

            if (remoteTask is TestAssemblyTask)
                return RunAssemblyTask(appDomain, remoteTask as TestAssemblyTask);
            if (remoteTask is TestClassTask)
                return RunClassTask(appDomain, remoteTask as TestClassTask);
            if (remoteTask is TestMethodTask)
                return RunMethodTask(appDomain, remoteTask as TestMethodTask);

            server.TaskOutput(remoteTask, "Unknown task type.", TaskOutputType.STDERR);
            return false;
        }

        private bool RunAssemblyTask(AppDomainWrapper appDomain, TestAssemblyTask task)
        {
            remoteRunner = appDomain.CreateObject<IRemoteRunner>(
                AssemblyName.GetAssemblyName("RemoteTestRunner.dll").FullName,
                "RemoteTestRunner.TestRunner");
            return true;
        }

        private bool RunClassTask(AppDomainWrapper appDomain, TestClassTask task)
        {
            return true;
        }

        private bool RunMethodTask(AppDomainWrapper appDomain, TestMethodTask task)
        {
            if (remoteRunner == null)
            {
                server.TaskOutput(task, "FixieRemoteRunner not instantiated.", TaskOutputType.STDERR);
                return false;
            }

            var setup = new TestSetup(task.AssemblyLocation, task.TypeName, task.MethodName);
            var result = remoteRunner.RunTest(setup);

            server.TaskOutput(task, result.Output, TaskOutputType.STDOUT);
            server.TaskDuration(task, result.Duration);
            if (result.Exceptions != null && !result.Exceptions.IsEmpty())
            {
                server.TaskException(task, ConvertExceptions(result.Exceptions));
            }
            
            return result.Pass;
        }

        private TaskException[] ConvertExceptions(IEnumerable<IException> exceptions)
        {
            return exceptions.Select(x => new TaskException(x.Type, x.Message, x.StackTrace)).ToArray();
        }
    }
}