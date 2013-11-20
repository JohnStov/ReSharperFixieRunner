using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class FixieTaskRunner : RecursiveRemoteTaskRunner
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
