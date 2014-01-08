using FixiePlugin.Elements;
using JetBrains.CommonControls;
using JetBrains.ReSharper.Feature.Services.Tree;
using JetBrains.ReSharper.Features.Shared.TreePsiBrowser;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.UnitTestFramework;
using JetBrains.TreeModels;
using JetBrains.UI.TreeView;

namespace FixiePlugin
{
    [UnitTestPresenter]
    public class TestClassPresenter : IUnitTestPresenter
    {
        private readonly TreePresenter treePresenter = new TreePresenter();

        public void Present(IUnitTestElement element, IPresentableItem item, TreeModelNode node, PresentationState state)
        {
            if (!(element is TestClassElement))
                return;
            treePresenter.UpdateItem(element, node, item, state);
        }

        private class TreePresenter : TreeModelBrowserPresenter
        {
            public TreePresenter()
            {
                Present(new PresentationCallback<TreeModelNode, IPresentableItem, TestClassElement>(PresentClassElement));
            }

            protected override bool IsNaturalParent(object parentValue, object childValue)
            {
                var unitTestNamespace = parentValue as IUnitTestNamespace;
                var classElement = childValue as TestClassElement;
                if (classElement != null && unitTestNamespace != null)
                    return unitTestNamespace.NamespaceName.Equals(classElement.GetNamespace().NamespaceName);
                return base.IsNaturalParent(parentValue, childValue);
            }

            protected override object Unwrap(object value)
            {
                var element = value as TestClassElement;
                if (element != null)
                    value = element.GetDeclaredElement();

                return base.Unwrap(value);
            }

            private void PresentClassElement(TestClassElement value, IPresentableItem item, TreeModelNode modelNode, PresentationState state)
            {
                if (IsNodeParentNatural(modelNode, value))
                    item.RichText = value.TypeName.ShortName;
                else if (string.IsNullOrEmpty(value.TypeName.GetNamespaceName()))
                    item.RichText = value.TypeName.ShortName;
                else
                    item.RichText = string.Format("{0}.{1}", value.TypeName.GetNamespaceName(), value.TypeName.ShortName);
            }
        }
    }
}

namespace JetBrains.ReSharper.Features.Shared.TreePsiBrowser
{
}

namespace JetBrains.ReSharper.Feature.Services.Tree
{
}
