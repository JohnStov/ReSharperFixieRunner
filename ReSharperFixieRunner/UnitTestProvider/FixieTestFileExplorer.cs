using System;

using JetBrains.Application;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [FileUnitTestExplorer]
    public class FixieTestFileExplorer : IUnitTestFileExplorer
    {
        private readonly FixieTestProvider provider;
        private readonly UnitTestElementFactory unitTestElementFactory;

        public FixieTestFileExplorer(FixieTestProvider provider, UnitTestElementFactory unitTestElementFactory)
        {
            this.provider = provider;
            this.unitTestElementFactory = unitTestElementFactory;
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            if (interrupted())
                return;

            psiFile.ProcessDescendants(new FixiePsiFileExplorer(unitTestElementFactory, consumer, psiFile, interrupted));
        }
        
        public IUnitTestProvider Provider { get { return provider; } }
    }
}