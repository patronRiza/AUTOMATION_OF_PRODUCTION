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
        private readonly NXLogger _logger;

        public PostprocessConfigurator(NXOpen.UI ui, NXLogger logger, Session session)
        {
            _logger = logger;
            _dialog = new PostprocessDialog(session, ui, logger);
        }

        public (bool Success, string PostName, string Extention, string OutputDir) Configure()
        {
            try
            {
                var result = _dialog.Launch();
                if (result != BlockDialog.DialogResponse.Ok)
                {
                    _logger.LogInfo("Постпроцессинг отменён пользователем");
                    return (false, null, null, null);
                }

                if (string.IsNullOrEmpty(_dialog.GetSelectedPostprocessor()))
                {
                    _logger.LogWarning("Постпроцессор не выбран");
                    return (false, null, null, null);
                }

                if (string.IsNullOrEmpty(_dialog.GetPostprocessorExtention()))
                {
                    _logger.LogWarning("Расширение не найдено");
                    return (false, null, null, null);
                }

                return (true, _dialog.GetSelectedPostprocessor(), _dialog.GetPostprocessorExtention(), _dialog.GetOutputDirectory());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                return (false, null, null, null);
            }
        }
    }
}
