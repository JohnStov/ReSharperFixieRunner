using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieRunner.TestRunner
{
    class FixieTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Fixie";

        private IRemoteTaskServer taskServer;

        public FixieTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            taskServer = server;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
        }
    }
}
