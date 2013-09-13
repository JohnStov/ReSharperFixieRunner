using JetBrains.ReSharper.Psi;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public static class PsiUnitTestIdentifier
    {
        public static bool IsAnyUnitTestElement(this IDeclaredElement element)
        {
            return IsDirectUnitTestClass(element as IClass) ||
                   IsContainingUnitTestClass(element as IClass) ||
                   IsUnitTestMethod(element as IMethod) ||
                   IsUnitTestDataProperty(element) ||
                   IsUnitTestClassConstructor(element);
        }

        public static bool IsUnitTest(this IDeclaredElement element)
        {
            return IsUnitTestMethod(element as IMethod);
        }

        public static bool IsUnitTestContainer(this IDeclaredElement element)
        {
            return IsDirectUnitTestClass(element as IClass);
        }

        private static bool IsDirectUnitTestClass(IClass @class)
        {
            return @class != null;
        }

        private static bool IsContainingUnitTestClass(IClass @class)
        {
            return false;
        }

        private static bool IsUnitTestMethod(IMethod method)
        {
            return method != null;
        }

        private static bool IsUnitTestDataProperty(IDeclaredElement element)
        {
            return false;
        }

        private static bool IsUnitTestClassConstructor(IDeclaredElement element)
        {
            return false;
        }
    }
}
