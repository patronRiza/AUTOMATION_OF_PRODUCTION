using NXOpen.CAM;
using System;
using System.IO;
using Operation = NXOpen.CAM.Operation;

namespace AutomationOfPostprocessing.Services.CAM
{
    public class OperationValidator
    {
        private OperationValidator() {}

        //public static bool ValidateBefore(Session session, CAMObject[] operations)
        //{
        //    //if (op == null)
        //    //{
        //    //    _logger.LogWarning("Обнаружена пустая операция");
        //    //    return false;
        //    //}

        //    //var status = op.GetStatus();
        //    //if (status == CAMObject.Status.Regen)
        //    //{
        //    //    _logger.LogWarning($"Операция {op.Name} не готова (статус: {status})");
        //    //    return false;
        //    //}

        //    //return true;

        //    bool allComplete = true;
        //    foreach (Operation op in operations)
        //    {

        //        if (op == null)
        //        {

        //            session.ListingWindow.WriteLine($"Предупреждение: {op.Name} пустая операция");
        //            continue;
        //        }

        //        var status = op.GetStatus();
        //        if (status == CAMObject.Status.Regen)
        //        {
        //            session.ListingWindow.WriteLine($"Внимание: Операция {op.Name} не была успешно рассчитана(статус: {status})");
        //            allComplete = false;
        //        }
        //    }

        //    return allComplete;
        //}

        public static ProcessingResult ValidateBefore(Operation operation)
        {
            if (operation == null)
                return ProcessingResult.Skipped("Предупреждение: Обнаружена пустая операция");
            

            try
            {
                var status = operation.GetStatus();
                if (status == CAMObject.Status.Regen)
                    return ProcessingResult.Skipped($"Внимание: Операция {operation.Name} не была успешно рассчитана(статус: {status})");

                return ProcessingResult.Success();
            }
            catch (Exception ex)
            {
                return ProcessingResult.Failed(ex);
            }
        }

        public static ProcessingResult ValidateAfter(string outputFile)
        {
            try
            {
                if (!File.Exists(outputFile))
                    return ProcessingResult.Failed(new FileNotFoundException($"Файл УП {outputFile} не был создан"));

                var fileInfo = new FileInfo(outputFile);
                return ProcessingResult.Success()
                    .WithAdditionalInfo("OutputFileSize", fileInfo.Length)
                    .WithAdditionalInfo("OutputFile", fileInfo);
            }
            catch(Exception ex)
            {
                return ProcessingResult.Failed(ex);
            }
        }
    }
}
