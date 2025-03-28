using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing
{
    public class ProcessingResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public Exception Error { get; private set; }

        private ProcessingResult() { }

        public static ProcessingResult Success()
        {
            return new ProcessingResult { IsSuccess = true };
        }

        public static ProcessingResult Skipped(string reason)
        {
            return new ProcessingResult { Message = reason };
        }

        public static ProcessingResult Failed(Exception ex)
        {
            return new ProcessingResult { Error = ex };
        }
    }
}
