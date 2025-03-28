using NXOpen.BlockStyler;
using NXOpen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationOfPostprocessing.UI.Notifiers;

namespace AutomationOfPostprocessing
{
    public class PostprocessConfigurator
    {
        private readonly PostprocessDialog _dialog;
        private readonly IUserNotifier _notifier;

        public PostprocessConfigurator(NXOpen.UI ui, IUserNotifier notifier, Session session)
        {
            _dialog = new PostprocessDialog(session, ui);
            _notifier = notifier;
        }

        public (bool Success, string PostName, string OutputDir) Configure()
        {
            try
            {
                var result = _dialog.Launch();
                if (result != BlockDialog.DialogResponse.Ok)
                {
                    _notifier.ShowInfo("Постпроцессинг отменён пользователем");
                    return (false, null, null);
                }

                if (string.IsNullOrEmpty(_dialog.GetSelectedPostprocessor()))
                {
                    _notifier.ShowError("Постпроцессор не выбран");
                    return (false, null, null);
                }

                return (true, _dialog.GetSelectedPostprocessor(), _dialog.GetOutputDirectory());
            }
            catch (Exception ex)
            {
                _notifier.ShowError($"Ошибка конфигурации: {ex.Message}");
                return (false, null, null);
            }
        }
    }
}
