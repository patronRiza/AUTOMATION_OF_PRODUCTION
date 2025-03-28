using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing
{
    public class OutputManager
    {
        private readonly string _basePath;

        public OutputManager(string basePath = null)
        {
            _basePath = basePath ?? Environment.GetEnvironmentVariable("UGII_RESULT_DIR");
        }

        public string PrepareOutputDirectory(string programName)
        {
            string path = Path.Combine(_basePath, programName);
            Directory.CreateDirectory(path);
            return path;
        }

        public string GenerateOutputFilename(string programName)
        {
            return Path.Combine(_basePath, $"{programName}.mpf");
        }
    }
}
