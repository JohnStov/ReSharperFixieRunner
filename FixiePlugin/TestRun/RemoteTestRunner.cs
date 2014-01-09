using System;
using System.Linq;
using System.Reflection;
using FixiePlugin.Tasks;

namespace FixiePlugin.TestRun
{
    public class RemoteTestRunner : MarshalByRefObject
    {
        private readonly string fixieAssemblyPath;

        public RemoteTestRunner(string fixiePath)
        {
            fixieAssemblyPath = fixiePath;

        }

        public string RunTest(ref TestMethodTask task)
        {
            var fixieAssembly = Assembly.LoadFrom(fixieAssemblyPath);
            var runnerType = fixieAssembly.GetExportedTypes().First(t => t.FullName = "Fixie.Runner");
            
            var listener = new FixieListener();
            var runner = (dynamic)Activator.CreateInstance(runnerType)

            var server = 
            
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
    }
}
