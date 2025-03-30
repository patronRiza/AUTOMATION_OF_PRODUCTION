using NXOpen.CAM;
using System;
using System.IO;
using Operation = NXOpen.CAM.Operation;

namespace AutomationOfPostprocessing.Services.CAM
{
    public class CAMValidator
    {
        private CAMValidator() {}

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
