using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace FixiePlugin.Actions
{
  [ActionHandler("ReSharperFixieTestProvider.Actions.About")]
  public class AboutAction : IActionHandler
  {
    public bool Update(IDataContext context, ActionPresentation presentation, DelegateUpdate nextUpdate)
    {
      // return true or false to enable/disable this action
      return true;
    }

    public void Execute(IDataContext context, DelegateExecute nextExecute)
    {
      MessageBox.Show(
        "ReSharperFixieTestProvider\n(c) John Stovin 2013\n\nA Unit Test plugin for the Fixie test framework",
        "About ReSharperFixieTestProvider",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }
  }
}
