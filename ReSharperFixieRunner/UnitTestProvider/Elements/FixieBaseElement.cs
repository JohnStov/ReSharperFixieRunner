using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Strategy;

namespace ReSharperFixieRunner.UnitTestProvider.Elements
{
    public abstract class FixieBaseElement : IUnitTestElement
    {
        private IUnitTestElement parent;
        private readonly ProjectModelElementEnvoy projectModelElementEnvoy;

        protected FixieBaseElement(FixieTestProvider provider, IUnitTestElement parent, string id, ProjectModelElementEnvoy projectModelElementEnvoy)
        {
            this.projectModelElementEnvoy = projectModelElementEnvoy;

            Provider = provider;
            Parent = parent;
            Id = id;

            Children = new List<IUnitTestElement>();
            ExplicitReason = string.Empty;

            SetState(UnitTestElementState.Valid);
        }

        // Simply to get around the virtual call in ctor warning
        protected void SetState(UnitTestElementState state)
        {
            State = state;
        }

        public abstract bool Equals(IUnitTestElement other);

        public IProject GetProject()
        {
            return projectModelElementEnvoy.GetValidProjectElement() as IProject;
        }


        public abstract string GetPresentation(IUnitTestElement parent = null);

        public abstract UnitTestNamespace GetNamespace();

        public abstract UnitTestElementDisposition GetDisposition();

        public abstract IDeclaredElement GetDeclaredElement();

        public abstract IEnumerable<IProjectFile> GetProjectFiles();

        public abstract IList<UnitTestTask> GetTaskSequence(ICollection<IUnitTestElement> explicitElements, IUnitTestLaunch launch);

        public abstract string Kind { get; }

        // TODO: use a real run strategy when we have a test runner
        private static readonly IUnitTestRunStrategy RunStrategy = new DoNothingRunStrategy();
        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return RunStrategy;
        }

        public IEnumerable<UnitTestElementCategory> Categories { get { yield break; } }

        public string ExplicitReason { get; private set; }

        public string Id { get; private set; }
        
        public IUnitTestProvider Provider { get; private set; }
        
        public IUnitTestElement Parent
        {
            get { return parent; }
            set
            {
                if (parent == value)
                    return;

                if (parent != null)
                    parent.Children.Remove(this);
                parent = value;
                if (parent != null)
                    parent.Children.Add(this);
            }
        }

        public ICollection<IUnitTestElement> Children { get; private set; }
        public string ShortName { get; protected set; }
        public bool Explicit { get { return !string.IsNullOrEmpty(ExplicitReason); } }
        public virtual UnitTestElementState State { get; set; }
    }
}