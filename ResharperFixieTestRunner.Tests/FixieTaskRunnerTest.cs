using JetBrains.ReSharper.TaskRunnerFramework;

using NSubstitute;

using ReSharperFixieTestRunner;

using Xunit;

namespace ResharperFixieTestRunner.Tests
{
    public class FixieTaskRunnerTest
    {
        [Fact]
        public void CanExecuteRecursive()
        {
            var remoteTaskServer = Substitute.For<IRemoteTaskServer>();

            var taskRunner = new TaskRunner(remoteTaskServer);
            var remoteTask = Substitute.For<RemoteTask>("RemoteTask");
            var node = new TaskExecutionNode(null, remoteTask);
            taskRunner.ExecuteRecursive(node);

            remoteTaskServer.Received().TaskStarting(Arg.Any<RemoteTask>());
            remoteTaskServer.Received().TaskFinished(Arg.Any<RemoteTask>(), Arg.Any<string>(), TaskResult.Success);
        }
    }
}
