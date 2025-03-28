
using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing.Services.CAM
{
    public class OperationValidator
    {
        private readonly NXLogger _logger;

        public OperationValidator(NXLogger logger) => _logger = logger;

        public bool Validate(Operation op)
        {
            if (op == null)
            {
                _logger.Log("Обнаружена пустая операция");
                return false;
            }

            var status = op.GetStatus();
            if (status != CAMObject.Status.Complete)
            {
                _logger.Log($"Операция {op.Name} не готова (статус: {status})");
                return false;
            }

            return true;
        }
    }
}
