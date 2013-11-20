using System.Linq;

using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperFixieTestProvider
{
    [FileUnitTestExplorer]
    public class FixieTestFileExplorer : IUnitTestFileExplorer
    {
        private readonly FixieTestProvider provider;
        private readonly UnitTestElementFactory unitTestElementFactory;
        private readonly FixieConventionCheck conventionCheck;

        public FixieTestFileExplorer(FixieTestProvider provider, UnitTestElementFactory unitTestElementFactory, FixieConventionCheck conventionCheck)
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

            psiFile.ProcessDescendants(new FixiePsiFileExplorer(unitTestElementFactory, conventionCheck, consumer, psiFile, interrupted));
        }
        
        public IUnitTestProvider Provider { get { return provider; } }
    }
}