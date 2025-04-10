using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutomationOfPostprocessing.Services.FileSystem
{
    public class PostprocessorService
    {
        private readonly string _postDir;
        private string _configFile;
        private string _actualPostsDir;

        public PostprocessorService()
        {
            _postDir = Environment.GetEnvironmentVariable("UGII_CAM_TEMPLATE_POST_DIR");
            _actualPostsDir = Environment.GetEnvironmentVariable("UGII_CAM_INSTALLED_POSTS_DIR");
        }

        public List<string> GetAvailablePostprocessors()
        {
            var posts = new List<string>();

            _configFile = Path.Combine(_postDir, "template_post.dat");

            try
            {
                foreach (string line in File.ReadLines(_configFile))
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

        public string GetExtention(string postName)
        {
            if (string.IsNullOrWhiteSpace(postName))
                throw new ArgumentException("Имя постпроцессора не может быть пустым", nameof(postName));

            try
            {
                string postFile = GetPostprocessorPaths(postName);

                if (!File.Exists(postFile))
                    throw new FileNotFoundException("Файл постпроцессора не найден");

                foreach (string line in File.ReadLines(postFile))
                {
                    if (line.Trim().StartsWith("Listfileflag_ncfile_ext"))
                    {
                        var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length >= 2)
                        {
                            return parts.Last().Trim();
                        }
                    }
                }

                throw new InvalidOperationException($"Не удалось найти расширение файла для постпроцессора {postName}");
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при загрузке расширения файла постпроцессора", ex);
            }
        }

        private string GetPostprocessorPaths(string postName)
        {
            try
            {
                foreach (string line in File.ReadLines(_configFile))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length < 3)
                        continue;

                    string currentPostName = parts[0].Trim();
                    if (currentPostName.Equals(postName, StringComparison.OrdinalIgnoreCase))
                    {
                        string path = parts[1].Trim().Replace("${UGII_CAM_INSTALLED_POSTS_DIR}", _actualPostsDir);

                        return path.Replace(".tcl", ".pui");
                    }
                }
                throw new KeyNotFoundException($"Постпроцессор {postName} не найден в конфигурационном файле");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении путей для постпроцессора {postName}", ex);
            }
        }
    }
}
