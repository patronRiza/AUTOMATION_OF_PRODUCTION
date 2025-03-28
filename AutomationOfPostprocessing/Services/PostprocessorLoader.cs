using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationOfPostprocessing.Services.FileSystem
{
    public class PostprocessorLoader
    {
        private readonly string _postDir;

        public PostprocessorLoader()
        {
            _postDir = Environment.GetEnvironmentVariable("UGII_CAM_POST_DIR");
        }

        public List<string> GetAvailablePostprocessors()
        {
            var posts = new List<string>();

            try
            {
                string configFile = Path.Combine(_postDir, "template_post.dat");
                foreach(string line in File.ReadLines(configFile))
                {
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                    {
                        string postName = line.Split(',').FirstOrDefault()?.Trim();
                        if (!string.IsNullOrEmpty(postName))
                        {
                            posts.Add(postName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке постпроцессоров", ex);
            }

            return posts;
        }
    }
}
