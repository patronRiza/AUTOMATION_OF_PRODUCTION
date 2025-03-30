using NXOpen.CAM;
using System;

namespace AutomationOfPostprocessing.Utils
{
    public class DataValidator
    {
        private readonly NXLogger _logger;

        public DataValidator(NXLogger logger)
        {
            _logger = logger;
        }

        public void ValidateParentGroup(NCGroup workPart)
        {
            if (workPart == null)
                throw new InvalidOperationException("Нет открытой рабочей части");
        }

        public void ValidatePostprocessConfigurator(bool success)
        {
            if (!success)
                throw new InvalidOperationException("Нет доступных программ");
        }
    }
}
