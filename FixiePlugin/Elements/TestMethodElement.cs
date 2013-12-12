using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using FixiePlugin.Tasks;
using FixiePlugin.TestDiscovery;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace FixiePlugin.Elements
{
    public class TestMethodElement : BaseElement, ISerializableUnitTestElement
    {
        private readonly DeclaredElementProvider declaredElementProvider;
        private readonly string methodName;
        private readonly string presentation;

        public TestMethodElement(TestProvider provider, IUnitTestElement parent, ProjectModelElementEnvoy projectModelElementEnvoy,
            DeclaredElementProvider declaredElementProvider, string id, IClrTypeName typeName, string methodName, string assemblyLocation)
            : base(provider, typeName, assemblyLocation, parent, id, projectModelElementEnvoy)
        {
            this.declaredElementProvider = declaredElementProvider;
            this.methodName = methodName;

            ShortName = methodName;
            presentation = string.Format("{0}.{1}", typeName.ShortName, methodName);
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as TestMethodElement);
        }

        private bool Equals(TestMethodElement other)
        {
            bool result;

            if (other == null)
                result = false;
            else 
                result = Equals(Id, other.Id) &&
                         Equals(TypeName, other.TypeName) &&
                         Equals(methodName, other.methodName) &&
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
            else if (obj.GetType() != typeof(TestMethodElement)) 
                result = false;
            else 
                result =  Equals((TestMethodElement)obj);

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (TypeName.FullName != null ? TypeName.FullName.GetHashCode() : 0);
                result = (result * 457) ^ (Id != null ? Id.GetHashCode() : 0);
                result = (result * 457) ^ (methodName != null ? methodName.GetHashCode() : 0);
                return result;
            }
        }

        public override string GetPresentation(IUnitTestElement parent)
        {
            return presentation;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return Parent != null ? Parent.GetNamespace() : new UnitTestNamespace(TypeName.GetNamespaceName());
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
            var declaredType = GetDeclaredType();
            if (declaredType == null)
                return null;

            // There is a small opportunity for this to choose the wrong method. If there is more than one
            // method with the same name (e.g. by error, or as an overload), this will arbitrarily choose the
            // first, whatever that means. Realistically, xunit throws an exception if there is more than
            // one method with the same name. We wouldn't know which one to go for anyway, unless we stored
            // the parameter types in this class. And that's overkill to fix such an edge case
            return (from member in declaredType.EnumerateMembers(methodName, declaredType.CaseSensistiveName)
                    where member is IMethod
                    select member).FirstOrDefault();
        }

        private ITypeElement GetDeclaredType()
        {
            return declaredElementProvider.GetDeclaredElement(GetProject(), TypeName) as ITypeElement;
        }

        public override IEnumerable<IProjectFile> GetProjectFiles()
        {
            var declaredType = GetDeclaredType();
            if (declaredType != null)
            {
                var result = (from sourceFile in declaredType.GetSourceFiles()
                              select sourceFile.ToProjectFile()).ToList<IProjectFile>();
                if (result.Count == 1)
                    return result;
            }

            var declaredElement = GetDeclaredElement();
            if (declaredElement == null)
                return EmptyArray<IProjectFile>.Instance;

            return from sourceFile in declaredElement.GetSourceFiles()
                   select sourceFile.ToProjectFile();
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            var sequence = TestClass.GetTaskSequence(explicitElements, launch);
            sequence.Add(new UnitTestTask(this, new TestMethodTask(TestClass.AssemblyLocation, TestClass.TypeName.FullName, ShortName)));
            return sequence;
        }

        public override string Kind
        {
            get { return "Fixie Test"; }
        }

        private TestClassElement TestClass
        {
            get { return Parent as TestClassElement; }
        }

        public void WriteToXml(XmlElement element)
        {
            element.SetAttribute("projectId", GetProject().GetPersistentID());
            element.SetAttribute("typeName", TypeName.FullName);
            element.SetAttribute("methodName", methodName);
        }

        internal static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, ISolution solution, UnitTestElementFactory unitTestElementFactory)
        {
            var testClass = parentElement as TestClassElement;
            if (testClass == null)
                throw new InvalidOperationException("parentElement should be Fixie test class");

            var typeName = parent.GetAttribute("typeName");
            var methodName = parent.GetAttribute("methodName");
            var projectId = parent.GetAttribute("projectId");

            var project = (IProject)ProjectUtil.FindProjectElementByPersistentID(solution, projectId);
            if (project == null)
                return null;

            return unitTestElementFactory.GetOrCreateTestMethod(project, testClass,
                typeName, methodName);
        }
    }
}
