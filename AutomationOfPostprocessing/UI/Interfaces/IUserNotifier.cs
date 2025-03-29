using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing.UI
{
    public interface IUserNotifier
    {
        void ShowInfo(string message);
        void ShowWarning(string message);
        void ShowError(string message);
    }
}
