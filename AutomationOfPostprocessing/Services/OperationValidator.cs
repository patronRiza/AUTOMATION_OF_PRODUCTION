
using NXOpen;
using NXOpen.CAE.AeroStructures.Author;
using NXOpen.CAM;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Operation = NXOpen.CAM.Operation;

namespace AutomationOfPostprocessing.Services.CAM
{
    public class OperationValidator
    {
        private OperationValidator() {}

        public static bool ValidateBefore(Session session, CAMObject[] operations)
        {
            //if (op == null)
            //{
            //    _logger.LogWarning("Обнаружена пустая операция");
            //    return false;
            //}

            //var status = op.GetStatus();
            //if (status == CAMObject.Status.Regen)
            //{
            //    _logger.LogWarning($"Операция {op.Name} не готова (статус: {status})");
            //    return false;
            //}

            //return true;

            bool allComplete = true;
            foreach (Operation op in operations)
            {
                
                if (op == null)
                {
                    session.ListingWindow.WriteLine($"Предупреждение: {op.Name} пустая операция");
                    continue;
                }

                var status = op.GetStatus();
                if (status == CAMObject.Status.Regen)
                {
                    session.ListingWindow.WriteLine($"Внимание: Операция {op.Name} не была успешно рассчитана(статус: {status})");
                    allComplete = false;
                }
            }

            return allComplete;
        }

        public static void ValidateAfter(Session session, string outputFile)
        {
            if (File.Exists(outputFile))
            {
                var fileInfo = new FileInfo(outputFile);
                session.ListingWindow.WriteLine("УП успешно сохранен в: " + outputFile);
                session.ListingWindow.WriteLine("Размер файла: " + fileInfo.Length + " байт");
            }
            else
            {
                session.ListingWindow.WriteLine($"Ошибка: Файл УП {outputFile} не был создан");
            }
        }
    }
}
