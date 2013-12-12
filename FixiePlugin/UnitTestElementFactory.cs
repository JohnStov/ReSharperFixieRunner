using FixiePlugin.Elements;
using FixiePlugin.TestDiscovery;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace FixiePlugin
{
    [SolutionComponent]
    public class UnitTestElementFactory
    {
        private readonly TestProvider provider;
        private readonly UnitTestElementManager unitTestManager;
        private readonly DeclaredElementProvider declaredElementProvider;

        public UnitTestElementFactory(TestProvider provider, UnitTestElementManager unitTestManager, DeclaredElementProvider declaredElementProvider)
        {
            this.provider = provider;
            this.unitTestManager = unitTestManager;
            this.declaredElementProvider = declaredElementProvider;
        }

        public TestClassElement GetOrCreateTestClass(
            IProject project,
            IClrTypeName typeName,
            string assemblyLocation)
        {
            var id = GetClassElementId(project, typeName);
            var element = unitTestManager.GetElementById(project, id);
            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
                var classElement = element as TestClassElement;
                return classElement;
            }

            return new TestClassElement(
                provider,
                new ProjectModelElementEnvoy(project),
                declaredElementProvider,
                id,
                typeName.GetPersistent(),
                assemblyLocation);
        }

        private static string GetClassElementId(IProject project, IClrTypeName typeName)
        {
            var id = string.Format("fixie:{0}:{1}", project.GetPersistentID(), typeName.FullName);
            return id;
        }

        public IUnitTestElement GetOrCreateTestMethod(IProject project, IClrTypeName typeName, string methodName, string assemblyLocation)
        {
            var classElementId = GetClassElementId(project, typeName);
            var classElement = unitTestManager.GetElementById(project, classElementId) as  TestClassElement;
            
            var id = string.Format("{0}.{1}", classElementId, methodName);
            var element = unitTestManager.GetElementById(project, id) as TestMethodElement;

            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
            }
            else
            {
                element = new TestMethodElement(
                    provider,
                    classElement,
                    new ProjectModelElementEnvoy(project),
                    declaredElementProvider,
                    id,
                    typeName.GetPersistent(),
                    methodName,
                    assemblyLocation);
            }

            if (!classElement.Children.Contains(element))
                classElement.Children.Add(element);

            return element;
        }

        public IUnitTestElement GetOrCreateTestMethod(IProject project, TestClassElement testClass, string methodName, string assemblyLocation)
        {
            return GetOrCreateTestMethod(project, testClass.TypeName, methodName, assemblyLocation);
        }
    }
}