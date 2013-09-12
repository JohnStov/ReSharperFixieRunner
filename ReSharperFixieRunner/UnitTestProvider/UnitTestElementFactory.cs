using System.Collections.Generic;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Elements;

namespace ReSharperFixieRunner.UnitTestProvider
{
    [SolutionComponent]
    public class UnitTestElementFactory
    {
        private readonly FixieTestProvider provider;
        private readonly UnitTestElementManager unitTestManager;
        private readonly DeclaredElementProvider declaredElementProvider;

        public UnitTestElementFactory(FixieTestProvider provider, UnitTestElementManager unitTestManager, DeclaredElementProvider declaredElementProvider)
        {
            this.provider = provider;
            this.unitTestManager = unitTestManager;
            this.declaredElementProvider = declaredElementProvider;
        }

        public FixieTestClassElement GetOrCreateTestClass(
            IProject project,
            IClrTypeName typeName,
            string assemblyLocation)
        {
            var id = string.Format("fixie:{0}:{1}", project.GetPersistentID(), typeName.FullName);
            var element = unitTestManager.GetElementById(project, id);
            if (element != null)
            {
                element.State = UnitTestElementState.Valid;
                var classElement = element as FixieTestClassElement;
                return classElement;
            }

            return new FixieTestClassElement(
                provider,
                new ProjectModelElementEnvoy(project),
                declaredElementProvider,
                id,
                typeName.GetPersistent(),
                assemblyLocation);
        }
    }
}