using System;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class TestRunnerHost
    {
        private TaskExecutionNode node;
        private IRemoteTaskServer server;

        public TestRunnerHost(TaskExecutionNode node, IRemoteTaskServer server)
        {
            this.node = node;
            this.server = server;
        }

        public void Run()
        {
            var task = node.RemoteTask as FixieTestAssemblyTask;

            var appDomainSetup = new AppDomainSetup
            {
                PrivateBinPath = task.AssemblyLocation,
                ShadowCopyFiles = "true",
            };

            var testRunner = new TestRunner();

            var appDomain = AppDomain.CreateDomain("FixieTestRunner", null, appDomainSetup);
            appDomain.DoCallBack(testRunner.RunTests);

            AppDomain.Unload(appDomain);
                
        }
    }
}