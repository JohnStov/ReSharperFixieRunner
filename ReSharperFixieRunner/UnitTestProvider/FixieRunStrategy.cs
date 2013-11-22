using System;
using JetBrains.DataFlow;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Strategy;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieRunStrategy : IUnitTestRunStrategy
    {
        public FixieRunStrategy()
        {
            
        }
        
        public RuntimeEnvironment GetRuntimeEnvironment(IUnitTestElement element, RuntimeEnvironment project,
            IUnitTestLaunch launch)
        {
            return RuntimeEnvironment.Automatic;
        }

        public void Run(Lifetime lifetime, ITaskRunnerHostController runController, IUnitTestRun run, IUnitTestLaunch launch,
            Action continuation)
        {
            continuation();
        }

        public void Cancel(ITaskRunnerHostController runController, IUnitTestRun run)
        {
        }

        public void Abort(ITaskRunnerHostController runController, IUnitTestRun run)
        {
        }

        public bool NeedProjectBuild(IProject project)
        {
            return false;
        }
    }
}