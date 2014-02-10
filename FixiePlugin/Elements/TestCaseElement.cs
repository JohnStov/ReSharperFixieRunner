using System;
using System.Collections.Generic;
using System.Xml;
using FixiePlugin.Tasks;

using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin.Elements
{
    public class TestCaseElement : BaseElement, ISerializableUnitTestElement
    {
        public TestCaseElement(TestProvider provider, BaseElement parent, ProjectModelElementEnvoy projectModelElementEnvoy,
            string id, string name)
            : base(provider, parent.TypeName, parent.AssemblyLocation, parent, id, projectModelElementEnvoy)
        {
            ShortName = name;

            SetState(UnitTestElementState.Dynamic);
        }

        public override bool Equals(IUnitTestElement other)
        {
            return Equals(other as TestCaseElement);
        }

        private bool Equals(TestCaseElement other)
        {
            bool result;

            if (other == null)
                result = false;
            else 
                result = Equals(Id, other.Id) &&
                         Equals(TypeName, other.TypeName) &&
                         Equals(AssemblyLocation, other.AssemblyLocation) &&
                         Equals(ShortName, other.ShortName);

            return result;
        }

        public override bool Equals(object obj)
        {
            bool result;

            if (ReferenceEquals(null, obj))
                result = false;
            else if (ReferenceEquals(this, obj)) 
                result = true;
            else if (obj.GetType() != typeof(TestCaseElement)) 
                result = false;
            else 
                result =  Equals((TestCaseElement)obj);

            return result;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var result = (TypeName.FullName != null ? TypeName.FullName.GetHashCode() : 0);
                result = (result * 457) ^ (Id != null ? Id.GetHashCode() : 0);
                result = (result * 457) ^ (ShortName != null ? ShortName.GetHashCode() : 0);
                return result;
            }
        }

        public override string GetPresentation(IUnitTestElement parent)
        {
            return ShortName;
        }

        public override UnitTestNamespace GetNamespace()
        {
            return Parent != null ? Parent.GetNamespace() : new UnitTestNamespace(TypeName.GetNamespaceName());
        }

        public override UnitTestElementDisposition GetDisposition()
        {
            return Parent == null ? null : Parent.GetDisposition();
        }

        public override IDeclaredElement GetDeclaredElement()
        {
            return Parent == null ? null : Parent.GetDeclaredElement();
        }

        public override IEnumerable<IProjectFile> GetProjectFiles()
        {
            return Parent == null ? null : Parent.GetProjectFiles();
        }

        public override IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch)
        {
            var sequence = Parent.GetTaskSequence(explicitElements, launch);
            sequence.Add(new UnitTestTask(this, new TestCaseTask(ShortName, ((BaseElement)Parent).AssemblyLocation)));
            return sequence;
        }

        public override string Kind
        {
            get { return "Fixie Test"; }
        }

        public void WriteToXml(XmlElement element)
        {
            element.SetAttribute(AttributeNames.ShortName, ShortName);
        }

        internal static IUnitTestElement ReadFromXml(XmlElement parent, IUnitTestElement parentElement, ISolution solution, UnitTestElementFactory unitTestElementFactory)
        {
            var testMethod = parentElement as TestMethodElement;
            if (testMethod == null)
                throw new InvalidOperationException("parentElement should be Fixie test method");

            var name = parent.GetAttribute(AttributeNames.ShortName);

            var project = testMethod.GetProject();
            if (project == null)
                return null;

            return unitTestElementFactory.GetOrCreateTestCase(project, testMethod, name);
        }
    }
}
