using NXOpen;
using AutomationOfPostprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NXOpen.CAM;
using Operation = NXOpen.CAM.Operation;
using System.IO;
using NXOpen.Gateway;

namespace AutomationOfPostprocessing.Services.CAM
{
    public class ProgramProcessor
    {
        private readonly Session _session;
        private readonly NXLogger _logger;

        public ProgramProcessor(Session session, NXLogger logger)
        {
            _session = session;
            _logger = logger;
        }

        public void ProcessPrograms(Dictionary<string, List<CAMObject>> programData,
                                          string outputDirectory, string postName)
        {
            foreach (var prog in programData)
            {
                string programName = prog.Key;
                string outputFile = OutputManager.GenerateOutputFilename(outputDirectory, programName);
                CAMObject[] operations = prog.Value.ToArray();

                _session.ListingWindow.WriteLine("\nОбработка программы: " + programName);

                try
                {
                    bool allValid = operations.All(op =>
                    {
                        var result = OperationValidator.ValidateBefore(op as Operation);

                        if (!result.IsSuccess && result.Message != null)
                            _session.ListingWindow.WriteLine(result.Message);

                        return result.IsSuccess;
                    });

                    if (!allValid)
                    {
                        _session.ListingWindow.WriteLine("Пропуск программы: не все операции рассчитаны");
                        continue;
                    }

                    _session.ListingWindow.WriteLine("Запуск постпроцессинга с постпроцессором: " + postName);

                    _session.Parts.Work.CAMSetup.PostprocessWithPostModeSetting(operations, postName, outputFile,
                                                                            CAMSetup.OutputUnits.PostDefined,
                                                                            CAMSetup.PostprocessSettingsOutputWarning.PostDefined,
                                                                            CAMSetup.PostprocessSettingsReviewTool.PostDefined,
                                                                            CAMSetup.PostprocessSettingsPostMode.Normal);

                    var validationResult = OperationValidator.ValidateAfter(outputFile);
                    if (validationResult.IsSuccess)
                    {
                        _session.ListingWindow.WriteLine("УП успешно сохранен в: " + outputFile);
                        _session.ListingWindow.WriteLine($"Размер файла: {validationResult.AdditionalInfo["OutputFileSize"]} байт");
                    }
                    else
                        _session.ListingWindow.WriteLine(validationResult.Error.Message);
                    
                }
                catch (Exception ex)
                {
                    _session.ListingWindow.WriteLine("Ошибка обработки программы: " + programName + " : " + ex.Message);
                    _logger.LogError(ex);
                }
            }
        }

    }
}