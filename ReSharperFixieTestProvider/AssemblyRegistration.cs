using JetBrains.ProjectModel;
using JetBrains.ReSharper.UnitTestFramework;

using ReSharperFixieTestRunner;

namespace ReSharperFixieTestProvider
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
