using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Fixie;

using FixiePlugin.Tasks;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace FixiePlugin.TestRun
{
    public class FixieListener : MarshalByRefObject, Listener
    {
        private readonly IRemoteTaskServer server;
        private readonly NodeRunner nodeRunner;
        private readonly bool isParameterized;

        public FixieListener(IRemoteTaskServer server, NodeRunner nodeRunner, bool isParameterized)
        {
            this.server = server;
            this.nodeRunner = nodeRunner;
            this.isParameterized = isParameterized;
        }

        public void AssemblyStarted(Assembly assembly)
        {
        }

        public void CaseSkipped(Case @case)
        {
        }

        public void CasePassed(PassResult result)
        {
            if (isParameterized)
            {
                var newTask = new TestCaseTask(result.Case.Name, nodeRunner.CurrentTask.AssemblyLocation);
                server.CreateDynamicElement(newTask);
                nodeRunner.AddTask(newTask);
            }

            var task = nodeRunner.CurrentTask;
            if (!string.IsNullOrWhiteSpace(result.Output))
                server.TaskOutput(task, result.Output, TaskOutputType.STDOUT);
            task.CloseTask(TaskResult.Success, result.Case.Name);

            if (isParameterized)
                nodeRunner.FinishCurrentTask(task);
        }

        public void CaseFailed(FailResult result)
        {
            if (isParameterized)
            {
                var newTask = new TestCaseTask(result.Case.Name, nodeRunner.CurrentTask.AssemblyLocation);
                server.CreateDynamicElement(newTask);
                nodeRunner.AddTask(newTask);
            }

            var task = nodeRunner.CurrentTask;
            if (!string.IsNullOrWhiteSpace(result.Output))
                server.TaskOutput(task, result.Output, TaskOutputType.STDOUT);
            if (result.Exceptions != null && result.Exceptions.Any())
            {
                server.TaskException(task, result.Exceptions.Select(ex => new TaskException(ex)).ToArray());
                task.CloseTask(TaskResult.Exception, result.Case.Name);
            }
            else
            {
                task.CloseTask(TaskResult.Error, result.Case.Name);
            }


            if (isParameterized)
                nodeRunner.FinishCurrentTask(task);
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
        }
    }
}