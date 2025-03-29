using AutomationOfPostprocessing.UI;
using AutomationOfPostprocessing.UI.Notifiers;
using NXOpen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NXOpen.ProductDemo;

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
            //File.AppendAllText(@"D:\NX_Logs\journal_log.txt", $"[{DateTime.Now}] - {message}" + Environment.NewLine);
        }

        public void LogWarning(string message)
        {
            LoggingIntoFile(message);
            _notifier.ShowWarning(message);
            //_notifier.ShowInfo($"[{DateTime.Now}] - {message}");
            //File.AppendAllText(@"D:\NX_Logs\journal_log.txt", $"[{DateTime.Now}] - {message}" + Environment.NewLine);
        }

        public void LogError(Exception ex)
        {
            string error = $"ERROR: {ex.Message}\n{ex.StackTrace}";
            //Log(error);
            LoggingIntoFile(error);
            _notifier.ShowError(error);
        }

        private void LoggingIntoFile(string message)
        {
            File.AppendAllText(_logpath, $"[{DateTime.Now}] - {message}" + Environment.NewLine);
        }
    }
}
