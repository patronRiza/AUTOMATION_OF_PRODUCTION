using System;
using System.IO;

namespace AutomationOfPostprocessing
{
    public class OutputManager
    {
        private OutputManager(){ }

        public static string GenerateOutputFilename(string outputDirectory, string programName, string extention)
        {
            string file = Path.Combine(outputDirectory, programName + "." + extention);
            
            if (File.Exists(file))
                File.Delete(file);

            return file;
        }
    }
}
