using System.Collections.Generic;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.ReSharper.UnitTestFramework.Strategy;

namespace ReSharperFixieRunner.UnitTestProvider.Elements
{
    public abstract class FixieBaseElement : IUnitTestElement
    {
        private readonly IUnitTestProvider provider;
        private IUnitTestElement parent;
        private readonly string id;
        private readonly ProjectModelElementEnvoy projectModelElementEnvoy;

        protected FixieBaseElement(FixieTestProvider provider, IUnitTestElement parent, string id, ProjectModelElementEnvoy projectModelElementEnvoy)
        {
            this.provider = provider;
            this.id = id;
            this.projectModelElementEnvoy = projectModelElementEnvoy;

            Parent = parent;

            Children = new List<IUnitTestElement>();

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

        private static readonly IUnitTestRunStrategy RunStrategy = new OutOfProcessUnitTestRunStrategy(new RemoteTaskRunnerInfo(FixieTaskRunner.RunnerId, typeof(FixieTaskRunner)));
        public IUnitTestRunStrategy GetRunStrategy(IHostProvider hostProvider)
        {
            return RunStrategy;
        }

        public IEnumerable<UnitTestElementCategory> Categories { get { yield break; } }

        public string ExplicitReason { get { return string.Empty; } }

        public string Id { get { return id; } }
        
        public IUnitTestProvider Provider { get {return provider; } }
        
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