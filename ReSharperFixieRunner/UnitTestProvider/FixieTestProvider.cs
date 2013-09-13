using JetBrains.Annotations;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

using ReSharperFixieRunner.UnitTestProvider.Elements;

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

        // Used to discover the type of the element - unknown, test, test container (class) or
        // something else relating to a test element (e.g. parent class of a nested test class)
        // This method is called to get the icon for the completion lists, amongst other things
        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    return !declaredElement.IsAnyUnitTestElement();

                case UnitTestElementKind.Test:
                    return declaredElement.IsUnitTest();

                case UnitTestElementKind.TestContainer:
                    return declaredElement.IsUnitTestContainer();

                case UnitTestElementKind.TestStuff:
                    return declaredElement.IsAnyUnitTestElement();
            }

            return false;
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    return !(element is FixieTestClassElement);

                case UnitTestElementKind.Test:
                    return false;

                case UnitTestElementKind.TestContainer:
                    return element is FixieTestClassElement;

                case UnitTestElementKind.TestStuff:
                    return element is FixieTestClassElement;
            }

            return false;
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return true;
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return 0;
        }

        public string ID { get { return "Fixie"; } }
        public string Name { get { return "Fixie"; } }
    }
}