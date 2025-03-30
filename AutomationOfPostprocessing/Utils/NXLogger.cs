using AutomationOfPostprocessing.UI;
using System;
using System.IO;

namespace AutomationOfPostprocessing
{
    public class NXLogger
    {
        private readonly IUserNotifier _notifier;
        private readonly string _logpath;
        private const long MaxLogSize = 10 * 1024 * 1024;

        public NXLogger(IUserNotifier userNotifier)
        {
            _notifier = userNotifier;
            _logpath = @"D:\NX_Logs\journal_log.txt";
            EnsureLogFileSize();
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
            try
            {
                File.AppendAllText(_logpath, $"[{DateTime.Now}] - {message}" + Environment.NewLine);
            }
            catch(Exception ex)
            {
                _notifier.ShowError($"Failed to write to log file: {ex.Message}");
            }
            
        }

        private void EnsureLogFileSize()
        {
            try
            {
                if(File.Exists(_logpath))
                {
                    FileInfo fileInfo = new FileInfo(_logpath);
                    if (fileInfo.Length > MaxLogSize)
                    {
                        File.Delete(_logpath);
                        File.WriteAllText(_logpath, $"[{DateTime.Now}] - Log file was rotated (size exceeded {MaxLogSize / 1024 / 1024} MB)\n");
                    }
                }
                else
                {
                    Directory.CreateDirectory(_logpath);
                    File.WriteAllText(_logpath, $"[{DateTime.Now}] - Log file was created\n");
                }
            }
            catch(Exception ex)
            {
                _notifier.ShowError($"Failed to manage log file: {ex.Message}");
            }
        }
    }
}
