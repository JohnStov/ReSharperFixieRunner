using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;

using Fixie;

using JetBrains.IDE.Resources;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class TestRunner
    {
        private readonly IRemoteTaskServer server;

        public TestRunner(IRemoteTaskServer server)
        {
            this.server = server;
        }

        public void Run(TaskExecutionNode node)
        {
                var task = node.RemoteTask;
                server.TaskStarting(task);
                var state = new TaskState(task);

                var xmlDoc = new XmlDocument();
                task.SaveXml(xmlDoc.DocumentElement);
                var xml = xmlDoc.ToDisplayString();

                if (task is FixieTestAssemblyTask)
                    RunAssembly(state, xml);
                else if (task is FixieTestClassTask)
                    RunClass(state, xml);
                else if (task is FixieTestMethodTask)
                    RunMethod(state, xml);

                foreach(var child in node.Children)
                    Run(child);
        }
            

        private void RunAssembly(TaskState state, string xml)
        {
            state.Message = "Assembly";
            state.Result = TaskResult.Success;
        }

        private void RunClass(TaskState state, string xml)
        {
            state.Message = "Class";
            state.Result = TaskResult.Success;
        }

        private void RunMethod(TaskState state, string xml)
        {
            var methodTask = state.Task as FixieTestMethodTask;
            if (methodTask != null)
            {
                var assembly = Assembly.LoadFile(methodTask.AssemblyLocation);
                var testType = assembly.GetType(methodTask.TypeName);
                var testMethod = testType.GetMethod(methodTask.MethodName);

                var listener = new ReSharperFixieTestListener(state);
                var runner = new Runner(listener);
                var startTime = DateTime.Now;
                runner.RunMethod(assembly, testMethod);
                state.Duration = DateTime.Now - startTime;
            }

        }
}

    public class TaskState
    {
        public TaskState(RemoteTask task)
        {
            Task = task;
            Duration = TimeSpan.Zero;
        }

        public RemoteTask Task { get; private set; }
        public TaskResult Result { get; set; }
        public string Message { get; set; }
        public TimeSpan Duration { get; set; }

    }

    public class ReSharperFixieTestListener : Listener
    {
        private readonly TaskState taskState;

        public ReSharperFixieTestListener(TaskState state)
        {
            taskState = state;
        }
        
        public void AssemblyStarted(Assembly assembly)
        {
        }

        public void CasePassed(PassResult result)
        {
            taskState.Result = TaskResult.Success;
            taskState.Message = result.Output;
        }

        public void CaseFailed(FailResult result)
        {
            taskState.Result = TaskResult.Exception;
            taskState.Message = result.Output;
        }

        public void AssemblyCompleted(Assembly assembly, AssemblyResult result)
        {
        }
    }
}