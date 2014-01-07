using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using Fixie;

using FixiePlugin.Tasks;
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
            var task = (FixieRemoteTask) node.RemoteTask;
            if (!(task is TestCaseTask))
            {
                AddTask(task);
                RunTask(task);
            }

            foreach (var child in node.Children)
                RunNode(child);

            FinishCurrentTask(task);
        }

        private void RunTask(FixieRemoteTask task)
        {
            if (task is TestAssemblyTask)
                RunAssemblyTask(task as TestAssemblyTask);
            else if (task is TestClassTask)
                RunClassTask(task as TestClassTask);
            else if (task is TestMethodTask)
                RunMethodTask(task as TestMethodTask);
            else if (task is TestCaseTask)
                RunCaseTask(task as TestCaseTask);
            else
            {
                server.TaskOutput(task, "Unknown task type.", TaskOutputType.STDERR);
                task.CloseTask(TaskResult.Error, "Unknown Task Type");
            }
        }

        private readonly Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
        
        private void RunAssemblyTask(TestAssemblyTask task)
        {
            var assemblyDir = Path.GetDirectoryName(task.AssemblyLocation);

            if (assemblyDir != null)
            {
                foreach (var file in Directory.EnumerateFiles(assemblyDir, "*.dll"))
                {
                    var assemblyPath = Path.Combine(assemblyDir, file);
                    var assembly = Assembly.LoadFile(assemblyPath);
                    assemblies.Add(assembly.FullName, assembly);
                }
            }
            
            task.CloseTask(TaskResult.Success, string.Empty);
        }

        private void RunClassTask(TestClassTask task)
        {
            task.CloseTask(TaskResult.Success, string.Empty);
        }

        private void RunMethodTask(TestMethodTask task)
        {
            var appDomain = AppDomain.CurrentDomain;
            appDomain.AssemblyResolve += OnAssemblyResolve;

            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(task.AssemblyLocation));

                var fixieAssemblyPath = Path.Combine(Path.GetDirectoryName(task.AssemblyLocation), "Fixie.dll");
                if (!IsRequiredFixieVersion(fixieAssemblyPath, RequiredFixieVersion.RequiredVersion))
                {
                    task.CloseTask(
                        TaskResult.Inconclusive,
                        string.Format("Test runner required Fixie version {0}", RequiredFixieVersion.RequiredVersion));
                    return;
                }

                var listener = new FixieListener(server, this, task.IsParameterized);
                var runner = new Runner(listener);

                var testAssembly = Assembly.LoadFile(task.AssemblyLocation);
                var testClass = testAssembly.GetType(task.TypeName);
                var testMethod = testClass.GetMethod(task.MethodName);

                var outcome = runner.RunMethod(testAssembly, testMethod);
                if (task.IsParameterized)
                {
                    TaskResult result = outcome.Failed == 0 ? TaskResult.Success : TaskResult.Error;
                    task.CloseTask(result, task.MethodName);
                }
            }
            catch (Exception ex)
            {
                task.CloseTask(TaskResult.Exception, ex.Message);
                FinishCurrentTask(task);
            }
            finally
            {
                appDomain.AssemblyResolve -= OnAssemblyResolve;
            }
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly resolved;
            assemblies.TryGetValue(args.Name, out resolved);
            
            return resolved;
        }

        private void RunCaseTask(TestCaseTask task)
        {
        }

        private bool IsRequiredFixieVersion(string fixieAssemblyPath, Version requiredVersion)
        {
            var assemblyName = AssemblyName.GetAssemblyName(fixieAssemblyPath);
            var version = assemblyName.Version;
            return version.CompareTo(requiredVersion) >= 0;
        }
    }
}