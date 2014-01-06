using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FixiePlugin.Tasks;
using FixiePlugin.TestDiscovery;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace FixiePlugin.Elements
{
    public class TestClassElement : BaseElement, ISerializableUnitTestElement
    {
        private readonly DeclaredElementProvider declaredElementProvider;

        public TestClassElement(TestProvider provider, ProjectModelElementEnvoy projectModelElementEnvoy, DeclaredElementProvider declaredElementProvider, string id, IClrTypeName typeName, string assemblyLocation)
            : base(provider, typeName, assemblyLocation, null, id, projectModelElementEnvoy)
        {
            this.declaredElementProvider = declaredElementProvider;

            ShortName = string.Join("+", typeName.TypeNames.Select(FormatTypeName).ToArray());
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as TestClassElement);
        }

        private bool Equals(TestClassElement other)
        {
            bool result;
            
            if (other == null)
                result = false;
            else
                result = Equals(Id, other.Id) &&
                         Equals(TypeName, other.TypeName) &&
                         Equals(AssemblyLocation, other.AssemblyLocation);

            return result;
        }

        public override bool Equals(object obj)
        {
            bool result;
            
            if (ReferenceEquals(null, obj))
                result = false;
            else if (ReferenceEquals(this, obj)) 
                result = true;
            else if (obj.GetType() != typeof(TestClassElement)) 
                result =  false;
            else
                result = Equals((TestClassElement)obj);

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (TypeName != null ? TypeName.GetHashCode() : 0);
                result = (result * 457) ^ (AssemblyLocation != null ? AssemblyLocation.GetHashCode() : 0);
                result = (result * 457) ^ (Id != null ? Id.GetHashCode() : 0);
                return result;
            }
        }

        public override string GetPresentation(IUnitTestElement parent = null)
        {
            return ShortName;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return new UnitTestNamespace(TypeName.GetNamespaceName());
        }

        public override UnitTestElementDisposition GetDisposition()
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

        public override IDeclaredElement GetDeclaredElement()
        {
            return declaredElementProvider.GetDeclaredElement(GetProject(), TypeName);
        }

        public override IEnumerable<IProjectFile> GetProjectFiles()
        {
            var declaredElement = GetDeclaredElement();
            if (declaredElement == null)
                return EmptyArray<IProjectFile>.Instance;

            return from sourceFile in declaredElement.GetSourceFiles()
                   select sourceFile.ToProjectFile();
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            return new List<UnitTestTask>
                       {
                           new UnitTestTask(null, new TestAssemblyTask(AssemblyLocation)),
                           new UnitTestTask(this, new TestClassTask(AssemblyLocation, TypeName.FullName))
                       };
        }

        public override string Kind
        {
            get { return "Fixie Test Class"; }
        }
 
        private static string FormatTypeName(TypeNameAndTypeParameterNumber typeName)
        {
            return typeName.TypeName + (typeName.TypeParametersNumber > 0 ? string.Format("`{0}", typeName.TypeParametersNumber) : string.Empty);
        }

        public void WriteToXml(XmlElement element)
        {
            element.SetAttribute(AttributeNames.ProjectId, GetProject().GetPersistentID());
            element.SetAttribute(AttributeNames.TypeName, TypeName.FullName);
        }

        internal static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, ISolution solution, UnitTestElementFactory unitTestElementFactory)
        {
            var projectId = parent.GetAttribute(AttributeNames.ProjectId);
            var typeName = parent.GetAttribute(AttributeNames.TypeName);

            var project = (IProject)ProjectUtil.FindProjectElementByPersistentID(solution, projectId);
            if (project == null)
                return null;
            var assemblyLocation = project.GetOutputFilePath().FullPath;

            return unitTestElementFactory.GetOrCreateTestClass(project, new ClrTypeName(typeName), assemblyLocation);
        }
    }
}