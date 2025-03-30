using AutomationOfPostprocessing.UI;
using System;
using System.IO;

namespace AutomationOfPostprocessing
{
    public class NXLogger
    {
        private readonly IUserNotifier _notifier;
        private readonly string _logpath;

        public NXLogger(IUserNotifier userNotifier)
        {
            _notifier = userNotifier;
            _logpath = @"D:\NX_Logs\journal_log.txt";
        }

        public void LogInfo(string message)
        {
            LoggingIntoFile(message);
            _notifier.ShowInfo(message);
        }

        public void LogWarning(string message)
        {
            LoggingIntoFile(message);
            _notifier.ShowWarning(message);
        }

        public void LogError(Exception ex)
        {
            string error = $"ERROR: {ex.Message}\n{ex.StackTrace}";
            LoggingIntoFile(error);
            //_notifier.ShowError(error);
        }

        private void LoggingIntoFile(string message)
        {
            File.AppendAllText(_logpath, $"[{DateTime.Now}] - {message}" + Environment.NewLine);
        }
    }
}
