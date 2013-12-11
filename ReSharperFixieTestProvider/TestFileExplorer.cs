using System.Linq;
using FixiePlugin.TestDiscovery;
using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin
{
    [FileUnitTestExplorer]
    public class TestFileExplorer : IUnitTestFileExplorer
    {
        private readonly TestProvider provider;
        private readonly UnitTestElementFactory unitTestElementFactory;
        private readonly TestIdentifier conventionCheck;

        public TestFileExplorer(TestProvider provider, UnitTestElementFactory unitTestElementFactory, TestIdentifier conventionCheck)
        {
            this.provider = provider;
            this.unitTestElementFactory = unitTestElementFactory;
            this.conventionCheck = conventionCheck;
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            if (interrupted())
                return;

            // don't bother going any further if there's isn't a project with a reference to the Fixie assembly
            var project = psiFile.GetProject();
            if (project == null)
                return;

            if(project.GetModuleReferences().All(module => module.Name != "Fixie"))
                return;

            psiFile.ProcessDescendants(new PsiFileExplorer(unitTestElementFactory, conventionCheck, consumer, psiFile, interrupted));
        }
        
        public IUnitTestProvider Provider { get { return provider; } }
    }
}