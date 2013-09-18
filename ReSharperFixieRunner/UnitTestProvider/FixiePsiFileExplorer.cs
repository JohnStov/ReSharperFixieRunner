using JetBrains.Application;
using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.Psi.Util;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.Util;

namespace ReSharperFixieRunner.UnitTestProvider
{
    public class FixiePsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly UnitTestElementFactory unitTestElementFactory;
        private readonly UnitTestElementLocationConsumer consumer;
        private readonly IFile psiFile;
        private readonly CheckForInterrupt interrupted;

        public FixiePsiFileExplorer(UnitTestElementFactory unitTestElementFactory, UnitTestElementLocationConsumer consumer, IFile psiFile, CheckForInterrupt interrupted)
        {
            this.unitTestElementFactory = unitTestElementFactory;
            this.consumer = consumer;
            this.psiFile = psiFile;
            this.interrupted = interrupted;
        }

        public bool InteriorShouldBeProcessed(ITreeNode element)
        {
            if (element is ITypeMemberDeclaration)
                return (element is ITypeDeclaration);

            return true;
        }

        public void ProcessBeforeInterior(ITreeNode element)
        {
            var declaration = element as IDeclaration;
            if (declaration == null)
                return;

            var declaredElement = declaration.DeclaredElement;
            if (declaredElement == null || declaredElement.ShortName == SharedImplUtil.MISSING_DECLARATION_NAME)
                return;

            IUnitTestElement testElement = null;
            var testClass = declaredElement as IClass;
            if (testClass != null)
                testElement = ProcessTestClass(testClass);

            var testMethod = declaredElement as IMethod;
            if (testMethod != null)
                testElement = ProcessTestMethod(testMethod);

            if (testElement != null)
            {
                var nameRange = declaration.GetNameDocumentRange().TextRange;
                var documentRange = declaration.GetDocumentRange().TextRange;
                if (nameRange.IsValid && documentRange.IsValid)
                {
                    var disposition = new UnitTestElementDisposition(testElement, psiFile.GetSourceFile().ToProjectFile(),
                                                                     nameRange, documentRange);
                    consumer(disposition);
                }
            }
        }

        public void ProcessAfterInterior(ITreeNode element)
        {
        }

        public bool ProcessingIsFinished
        {
            get
            {
                if (interrupted())
                    throw new ProcessCancelledException();

                return false;
            }
        }

        private IUnitTestElement ProcessTestClass(IClass testClass)
        {
            if (!IsValidTestClass(testClass))
                return null;

            var project = psiFile.GetProject();
            var clrTypeName = testClass.GetClrName();
            var assemblyPath = project.GetOutputFilePath().FullPath;
            return unitTestElementFactory.GetOrCreateTestClass(project, clrTypeName, assemblyPath);
        }

        private bool IsValidTestClass(IClass testClass)
        {
            return testClass != null 
                && testClass.GetAccessRights() == AccessRights.PUBLIC
                // IL marks static class as sealed && abstract, so abstract check will find static classes too
                && !testClass.IsAbstract
                && testClass.CanInstantiateWithPublicDefaultConstructor()
                && testClass.ShortName.EndsWith("Tests");
        }

        private IUnitTestElement ProcessTestMethod(IMethod testMethod)
        {
            var @class = testMethod.GetContainingType() as IClass;
            if (!IsValidTestClass(@class))
                return null;
            
            if (!IsValidTestMethod(testMethod))
                return null;

            var clrTypeName = @class.GetClrName();
            var project = psiFile.GetProject();
            var assemblyPath = project.GetOutputFilePath().FullPath;
            return unitTestElementFactory.GetOrCreateTestMethod(project, clrTypeName, testMethod.ShortName, assemblyPath);
        }

        private bool IsValidTestMethod(IMethod testMethod)
        {
            return testMethod.GetAccessRights() == AccessRights.PUBLIC 
                && !testMethod.IsAbstract 
                && !testMethod.IsStatic
                && testMethod.Parameters.Count == 0 
                && testMethod.ReturnType.IsVoid();
        }
    }
}