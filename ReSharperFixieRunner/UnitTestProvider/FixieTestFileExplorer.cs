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
        private readonly IUnitTestProvider provider;
        private readonly IUnitTestElementFactory unitTestElementFactory;

        public FixieTestFileExplorer(FixieTestProvider provider, UnitTestElementFactory unitTestElementFactory)
        {
            this.provider = provider;
            this.unitTestElementFactory = unitTestElementFactory;
        }

        private FixieTestFileExplorer(IUnitTestProvider provider, IUnitTestElementFactory unitTestElementFactory)
        {
            this.provider = provider;
            this.unitTestElementFactory = unitTestElementFactory;
        }

        public static FixieTestFileExplorer CreateForTest(
            IUnitTestProvider provider,
            IUnitTestElementFactory unitTestElementFactory)
        {
            return new FixieTestFileExplorer(provider, unitTestElementFactory);
        }

        public void ExploreFile(IFile psiFile, UnitTestElementLocationConsumer consumer, CheckForInterrupt interrupted)
        {
            if (psiFile == null)
                throw new ArgumentNullException("psiFile");

            if (interrupted())
                return;

            psiFile.ProcessDescendants(new FixiePsiFileExplorer(unitTestElementFactory, consumer, psiFile, interrupted));
        }
        
        public IUnitTestProvider Provider { get { return provider; } }
    }
}