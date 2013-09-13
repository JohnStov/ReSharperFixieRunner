using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;

using ReSharperFixieRunner.UnitTestProvider.Elements;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public interface IUnitTestElementFactory
    {
        FixieTestClassElement GetOrCreateTestClass(
            IProject project,
            IClrTypeName typeName,
            string assemblyLocation);
    }
}