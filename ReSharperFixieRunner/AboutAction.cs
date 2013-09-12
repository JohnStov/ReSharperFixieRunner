using System.Windows.Forms;
using JetBrains.ActionManagement;
using JetBrains.Application.DataContext;

namespace ReSharperFixieRunner
{
  [ActionHandler("ReSharperFixieRunner.About")]
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
        "ReSharperFixieRunner\nJohn Stovin\n\nA Unit Test plugin for the Fixie test framework",
        "About ReSharperFixieRunner",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }
  }
}
