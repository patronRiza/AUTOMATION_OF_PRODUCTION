using NXOpen.CAM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing
{
    public class OperationsValidatorMain
    {
        public ProcessingResult Process(Operation op)
        {
            try
            {
                if (op.GetStatus() != CAMObject.Status.Complete)
                    return ProcessingResult.Skipped("Операция не готова");



                return ProcessingResult.Success();
            }
            catch(Exception ex)
            {
                return ProcessingResult.Failed(ex);
            }
        }
    }
}
