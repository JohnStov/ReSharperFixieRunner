using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieTestRunner
{
    public class TaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Fixie";

        public TaskRunner(IRemoteTaskServer server)
            : base(server)
        {
        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
            var testRunnerHost = new TestRunnerHost(node, Server);

            testRunnerHost.Run();
        }
    }
}
