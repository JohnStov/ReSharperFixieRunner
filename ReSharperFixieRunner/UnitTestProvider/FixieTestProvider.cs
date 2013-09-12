using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [UnitTestProvider, UsedImplicitly]
    public class FixieTestProvider : IUnitTestProvider
    {
        public FixieTestProvider()
        {}

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            return false;
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            return false;
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return false;
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return 0;
        }

        public string ID { get { return "Fixie"; } }
        public string Name { get { return "Fixie"; } }
    }
}