using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JetBrains.ReSharper.TaskRunnerFramework;

namespace ReSharperFixieRunner
{
    public class FixieTaskRunner : RecursiveRemoteTaskRunner
    {
        public const string RunnerId = "Fixie";

        public FixieTaskRunner(IRemoteTaskServer server)
            : base(server)
        {

        }

        public override void ExecuteRecursive(TaskExecutionNode node)
        {
        }
    }
}
