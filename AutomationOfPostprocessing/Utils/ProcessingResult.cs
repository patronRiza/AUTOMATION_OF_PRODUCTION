using System;
using System.Collections.Generic;

namespace AutomationOfPostprocessing
{
    public class ProcessingResult
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public Exception Error { get; private set; }
        public Dictionary<string, object> AdditionalInfo { get; } = new Dictionary<string, object>();

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

        public ProcessingResult WithAdditionalInfo(string key, object value)
        {
            AdditionalInfo[key] = value;
            return this;
        }
    }
}
