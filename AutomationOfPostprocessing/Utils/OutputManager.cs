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
        private static string _basePath;

        private OutputManager(){ }

        private static void PrepareOutputDirectory()
        {
            _basePath = Environment.GetEnvironmentVariable("UGII_RESULT_DIR");
        }

        public static string GenerateOutputFilename(string outputDirectory, string programName)
        {
            PrepareOutputDirectory();
            string file = Path.Combine(outputDirectory, programName + ".mpf");
            //string file = Path.Combine(_basePath, $"{programName}.mpf");
            
            if (File.Exists(file))
                File.Delete(file);

            return file;
        }
    }
}
