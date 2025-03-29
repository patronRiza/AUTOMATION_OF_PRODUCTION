using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing.UI.Notifiers
{
    public class NXUserNotifier : IUserNotifier
    {
        private readonly NXOpen.UI _ui;

        public NXUserNotifier(NXOpen.UI ui) => _ui = ui;

        public void ShowError(string message)
        {
            _ui.NXMessageBox.Show("Информация", NXMessageBox.DialogType.Information, message);
        }

        public void ShowInfo(string message)
        {
            _ui.NXMessageBox.Show("Предупреждение", NXMessageBox.DialogType.Warning, message);
        }

        public void ShowWarning(string message)
        {
            _ui.NXMessageBox.Show("Ошибка", NXMessageBox.DialogType.Error, message);
        }
    }
}
