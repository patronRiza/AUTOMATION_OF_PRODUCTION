using NXOpen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing
{
    public class NXLogger
    {
        private readonly ListingWindow _lw;

        public NXLogger(ListingWindow lw) => _lw = lw;

        public void Log(string message)
        {
            _lw.WriteLine($"[{DateTime.Now}] - {message}");
            File.AppendAllText(@"D:\NX_Logs\journal_log.txt", $"[{DateTime.Now}] - {message}" + Environment.NewLine);
        }

        public void LogError(Exception ex)
        {
            string error = $"ERROR: {ex.Message}\n{ex.StackTrace}";
            Log(error);
        }
    }
}
