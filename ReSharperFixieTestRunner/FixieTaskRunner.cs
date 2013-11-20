using JetBrains.ReSharper.Psi.BuildScripts.Icons;
using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class FixieTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Fixie";

        private readonly IRemoteTaskServer taskServer;

        public FixieTaskRunner(IRemoteTaskServer server)
            : base(server)
        {
            taskServer = server;
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            taskServer.TaskStarting(node.RemoteTask);
            
            taskServer.TaskFinished(node.RemoteTask, "Task Finished", TaskResult.Success);
        }
    }
}
