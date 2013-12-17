using System.Collections.Generic;
using System.Linq;

using FixiePlugin.Elements;
using FixiePlugin.Tasks;
using JetBrains.Annotations;
using JetBrains.Application;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.TaskRunnerFramework;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin
{
    [UnitTestProvider]
    [UsedImplicitly]
    public class TestProvider : IUnitTestProvider, IDynamicUnitTestProvider
    {
        private static readonly UnitTestElementComparer Comparer =
            new UnitTestElementComparer(
                new[] { typeof(TestClassElement), typeof(TestMethodElement), typeof(TestCaseElement) });

        public void ExploreExternal(UnitTestElementConsumer consumer)
        {
        }

        public void ExploreSolution(ISolution solution, UnitTestElementConsumer consumer)
        {
        }

        // Used to discover the type of the element - unknown, test, test container (class) or
        // something else relating to a test element (e.g. parent class of a nested test class)
        // This method is called to get the icon for the completion lists, amongst other things
        public bool IsElementOfKind(IDeclaredElement declaredElement, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    return !declaredElement.IsAnyUnitTestElement();

                case UnitTestElementKind.Test:
                    return declaredElement.IsUnitTest();

                case UnitTestElementKind.TestContainer:
                    return declaredElement.IsUnitTestContainer();

                case UnitTestElementKind.TestStuff:
                    return declaredElement.IsAnyUnitTestElement();
            }

            return false;
        }

        public bool IsElementOfKind(IUnitTestElement element, UnitTestElementKind elementKind)
        {
            switch (elementKind)
            {
                case UnitTestElementKind.Unknown:
                    return !(element is BaseElement);

                case UnitTestElementKind.Test:
                    return element is TestMethodElement || element is TestCaseElement;

                case UnitTestElementKind.TestContainer:
                    return element is TestClassElement;

                case UnitTestElementKind.TestStuff:
                    return element is TestClassElement;
            }

            return false;
        }

        public bool IsSupported(IHostProvider hostProvider)
        {
            return true;
        }

        public int CompareUnitTestElements(IUnitTestElement x, IUnitTestElement y)
        {
            return Comparer.Compare(x, y);
        }

        public string ID
        {
            get
            {
                return "Fixie";
            }
        }

        public string Name
        {
            get
            {
                return "Fixie";
            }
        }

        public IUnitTestElement GetDynamicElement(RemoteTask remoteTask, Dictionary<RemoteTask, IUnitTestElement> tasks)
        {
            var caseTask = remoteTask as TestCaseTask;
            if (caseTask != null)
                return GetDynamicCaseElement(tasks, caseTask);

            return null;
        }

        private IUnitTestElement GetDynamicCaseElement(
            Dictionary<RemoteTask, IUnitTestElement> tasks,
            TestCaseTask caseTask)
        {
            var methodElement = 
                tasks.Where(kvp => kvp.Key is TestMethodTask && IsParentMethodTask(kvp.Key as TestMethodTask, caseTask))
                     .Select(kvp => kvp.Value).FirstOrDefault() as TestMethodElement;
            if (methodElement == null)
                return null;

            using (ReadLockCookie.Create())
            {
                var project = methodElement.GetProject();
                if (project == null)
                    return null;

                var element = UnitTestElementFactory.GetTestCase(
                    project,
                    methodElement,
                    caseTask.MethodName + caseTask.Parameters);

                // Make sure we return an element, even if the system already knows about it. If it's
                // part of the test run, it will already have been added in GetTaskSequence, and this
                // method (GetDynamicElement) doesn't get called. But if you try and run just a single
                // theory, Fixie will run ALL theories for that method, and will need the elements
                // for those theories not included in the task sequence. This is necessary because if
                // one of those theories throws an exception, UnitTestLaunch.TaskException doesn't
                // have an element to report against, and displays a message box
                if (element != null)
                {
                    // If the element is invalid, it's been removed from its parent, so add it back,
                    // and reset the state
                    if (element.State == UnitTestElementState.Invalid)
                    {
                        element.State = UnitTestElementState.Dynamic;
                        element.Parent = methodElement;
                    }
                    return element;
                }

                return UnitTestElementFactory.CreateTestCase(
                    this,
                    project,
                    methodElement,
                    caseTask.MethodName + caseTask.Parameters);
            }
        }

        private bool IsParentMethodTask(TestMethodTask methodTask, TestCaseTask caseTask)
        {
            return methodTask.AssemblyLocation == caseTask.AssemblyLocation && methodTask.TypeName == caseTask.TypeName
                   && methodTask.MethodName == caseTask.MethodName;
        }

    }
}