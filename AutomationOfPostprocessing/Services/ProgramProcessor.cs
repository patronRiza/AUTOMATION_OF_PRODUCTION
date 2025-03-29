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
                    //bool allComplete = true;
                    //foreach (Operation op in operations)
                    //{
                    //    if (op == null)
                    //    {
                    //        _session.ListingWindow.WriteLine("Предупреждение: Обнаружена пустая операция");
                    //        continue;
                    //    }

                    //    var status = op.GetStatus();
                    //    if (status == CAMObject.Status.Regen)
                    //    {
                    //        _session.ListingWindow.WriteLine("Внимание: Операция " + op.Name + " не была успешно рассчитана(статус: " + status + ")");
                    //        allComplete = false;
                    //    }
                    //}

                    //if (!allComplete)
                    //{
                    //    _session.ListingWindow.WriteLine("Пропуск программы: не все операции рассчитаны");
                    //    continue;
                    //}

                    bool allComplete = OperationValidator.ValidateBefore(_session, operations);
                    if (!allComplete)
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

                    OperationValidator.ValidateAfter(_session, outputFile);

                    //if (File.Exists(outputFile))
                    //{
                    //    var fileInfo = new FileInfo(outputFile);
                    //    _session.ListingWindow.WriteLine("УП успешно сохранен в: " + outputFile);
                    //    _session.ListingWindow.WriteLine("Размер файла: " + fileInfo.Length + " байт");
                    //}
                    //else
                    //{
                    //    _session.ListingWindow.WriteLine("Ошибка: Файл УП не был создан");
                    //}
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