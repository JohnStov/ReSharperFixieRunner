using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Fixie;

using FixiePlugin.Tasks;

using JetBrains.ProjectModel.Resources;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.TestRun
{
    public class NodeRunner
    {
        private readonly IRemoteTaskServer server;
        private readonly Stack<FixieRemoteTask> taskStack = new Stack<FixieRemoteTask>();

        public NodeRunner(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public FixieRemoteTask CurrentTask { get { return taskStack.Peek();} }

        public void AddTask(FixieRemoteTask task)
        {
            taskStack.Push(task);
            server.TaskStarting(task);
        }

        public void FinishCurrentTask(FixieRemoteTask expectedTask)
        {
            while (taskStack.Any())
            {
                var task = taskStack.Pop();
                server.TaskFinished(task, task.Message, task.TaskResult);
                if (task.Equals(expectedTask))
                    break;
            }
        }


        public void RunNode(TaskExecutionNode node)
        {
            RunTask((FixieRemoteTask)node.RemoteTask);

            foreach (var child in node.Children)
                RunNode(child);
        }

        private void RunTask(FixieRemoteTask task)
        {
            AddTask(task);

            if (task is TestAssemblyTask)
                RunAssemblyTask(task as TestAssemblyTask);
            else if (task is TestClassTask)
                RunClassTask(task as TestClassTask);
            else if (task is TestMethodTask)
                RunMethodTask(task as TestMethodTask);
            else if (task is ParameterizedTestMethodTask)
                RunParameterizedMethodTask(task as ParameterizedTestMethodTask);
            else
            {
                server.TaskOutput(task, "Unknown task type.", TaskOutputType.STDERR);
                task.CloseTask(TaskResult.Error, "Unknown Task Type");
            }

            FinishCurrentTask(task);

        }

        private void RunAssemblyTask(TestAssemblyTask task)
        {
            task.CloseTask(TaskResult.Success, string.Empty);
        }

        private void RunClassTask(TestClassTask task)
        {
            task.CloseTask(TaskResult.Success, string.Empty);
        }

        private void RunMethodTask(TestMethodTask task)
        {
            var listener = new FixieListener(server, this, task.IsParameterized);
            var runner = new Runner(listener);

            var testAssembly = Assembly.LoadFile(task.AssemblyLocation);
            var testClass = testAssembly.GetType(task.TypeName);
            var testMethod = testClass.GetMethod(task.MethodName);

            runner.RunMethod(testAssembly, testMethod);
        }

        private void RunParameterizedMethodTask(ParameterizedTestMethodTask task)
        {
            server.TaskOutput(task, "Should never run a ParameterizedTestMethodTask directly.", TaskOutputType.STDERR);
            task.CloseTask(TaskResult.Error, "Should never run a ParameterizedTestMethodTask directly");
        }


    }
}