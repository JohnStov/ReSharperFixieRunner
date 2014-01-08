using System;

using JetBrains.Application.Progress;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.ExtensionsAPI;
using JetBrains.ReSharper.Psi.Tree;
using JetBrains.ReSharper.UnitTestFramework;

namespace FixiePlugin.TestDiscovery
{
    public class PsiFileExplorer : IRecursiveElementProcessor
    {
        private readonly UnitTestElementFactory unitTestElementFactory;
        private readonly UnitTestElementLocationConsumer consumer;
        private readonly IFile psiFile;
        private readonly Func<bool> interrupted;
        private readonly TestIdentifier conventionCheck;

        public PsiFileExplorer(UnitTestElementFactory unitTestElementFactory, TestIdentifier conventionCheck, UnitTestElementLocationConsumer consumer, IFile psiFile, Func<bool> interrupted)
        {
            this.unitTestElementFactory = unitTestElementFactory;
            this.conventionCheck = conventionCheck;
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
            var project = psiFile.GetProject();
            if (!conventionCheck.IsValidTestClass(project, testClass))
                return null;

            var clrTypeName = testClass.GetClrName();
            var assemblyPath = project.GetOutputFilePath().FullPath;
            return unitTestElementFactory.GetOrCreateTestClass(project, clrTypeName, assemblyPath);
        }

        private IUnitTestElement ProcessTestMethod(IMethod testMethod)
        {
            var project = psiFile.GetProject();
            var testClass = testMethod.GetContainingType() as IClass;

            if (!conventionCheck.IsValidTestMethod(project, testClass, testMethod))
                return null;

            var isDynamic = conventionCheck.IsParameterizedMethod(project, testClass, testMethod);

            var clrTypeName = testClass.GetClrName();
            var assemblyPath = project.GetOutputFilePath().FullPath;
            return unitTestElementFactory.GetOrCreateTestMethod(project, clrTypeName, testMethod.ShortName, assemblyPath, isDynamic);
        }
    }
}