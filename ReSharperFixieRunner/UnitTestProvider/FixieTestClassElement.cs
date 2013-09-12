using System.Collections.Generic;
using System.Linq;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Strategy;
using JetBrains.Util;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixieTestClassElement : IUnitTestElement
    {
        private readonly ProjectModelElementEnvoy projectModelElementEnvoy;
        private readonly DeclaredElementProvider declaredElementProvider;
        private readonly IClrTypeName typeName;
        
        public FixieTestClassElement(FixieTestProvider provider, ProjectModelElementEnvoy projectModelElementEnvoy, DeclaredElementProvider declaredElementProvider, string id, IClrTypeName typeName, string assemblyLocation)
        {
            this.projectModelElementEnvoy = projectModelElementEnvoy;
            this.declaredElementProvider = declaredElementProvider;
            this.typeName = typeName;
            
            AssemblyLocation = assemblyLocation;
            Id = id;
            Provider = provider;
            Children = new List<IUnitTestElement>();

            ShortName = string.Join("+", typeName.TypeNames.Select(FormatTypeName).ToArray());
        }

        public bool Equals(IUnitTestElement other)
        {
            return Equals(other as FixieTestClassElement);
        }

        public bool Equals(FixieTestClassElement other)
        {
            if (other == null)
                return false;

            return Equals(Id, other.Id) &&
                   Equals(typeName, other.typeName) &&
                   Equals(AssemblyLocation, other.AssemblyLocation);
        }

        public IProject GetProject()
        {
            return projectModelElementEnvoy.GetValidProjectElement() as IProject;
        }

        public string GetPresentation(IUnitTestElement parent = null)
        {
            return ShortName;
        }

        public UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(typeName.GetNamespaceName());
        }

        public UnitTestElementDisposition GetDisposition()
        {
            var element = GetDeclaredElement();
            if (element == null || !element.IsValid())
                return UnitTestElementDisposition.InvalidDisposition;

            var locations = from declaration in element.GetDeclarations()
                            let file = declaration.GetContainingFile()
                            where file != null
                            select new UnitTestElementLocation(file.GetSourceFile().ToProjectFile(),
                                                               declaration.GetNameDocumentRange().TextRange,
                                                               declaration.GetDocumentRange().TextRange);
            return new UnitTestElementDisposition(locations, this);
        }

        public IDeclaredElement GetDeclaredElement()
        {
            return declaredElementProvider.GetDeclaredElement(GetProject(), typeName);
        }

        public IEnumerable<IProjectFile> GetProjectFiles()
        {
            var declaredElement = GetDeclaredElement();
            if (declaredElement == null)
                return EmptyArray<IProjectFile>.Instance;

            return from sourceFile in declaredElement.GetSourceFiles()
                   select sourceFile.ToProjectFile();
        }

        // TODO: use a real run strategy when we have a test runner
        private static readonly IUnitTestRunStrategy RunStrategy = new DoNothingRunStrategy();
        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return RunStrategy;
        }

        public IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            // TODO: Populate with useful tasks when we have them
            return new List<UnitTestTask>();
        }

        public string Kind
        {
            get { return "Fixie Test Class"; }
        }
 
        public IEnumerable<UnitTestElementCategory> Categories { get {yield break;} }
        
        public string ExplicitReason { get { return string.Empty; } }

        public string Id { get; private set; }
        
        public IUnitTestProvider Provider { get; private set; }

        public IUnitTestElement Parent
        {
            get { return null; }
            // TODO: Implement Parent properly for TestElement
            set {}
        }
        
        public ICollection<IUnitTestElement> Children { get; private set; }

        public string ShortName { get; private set; }
        
        public bool Explicit { get {return false; } }

        public UnitTestElementState State { get; set; }

        public string AssemblyLocation { get; private set; }

        private static string FormatTypeName(TypeNameAndTypeParameterNumber typeName)
        {
            return typeName.TypeName + (typeName.TypeParametersNumber > 0 ? string.Format("`{0}", typeName.TypeParametersNumber) : string.Empty);
        }
    }
}