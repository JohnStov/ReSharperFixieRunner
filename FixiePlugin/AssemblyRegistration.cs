using FixiePlugin.TestRun;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin
{
    [SolutionComponent]
    public class AssemblyRegistration
    {
        public AssemblyRegistration(UnitTestingAssemblyLoader assemblyLoader)
        {
            // Register assemblies needed later by test runner.
            assemblyLoader.RegisterAssembly(typeof(TaskRunner).Assembly);
        }
    }
}
