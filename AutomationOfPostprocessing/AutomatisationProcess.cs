using NXOpen;
using NXOpen.BlockStyler;
using NXOpen.CAE;
using NXOpen.CAM;
using NXOpen.UF;
using System;
using System.Collections.Generic;
using System.IO;
using Operation = NXOpen.CAM.Operation;

namespace LearnAutomatisationProcess
{
    public class AutomatisationProcess
    {
        private static Session theSession;
        private static UI theUI;
        private static UFSession theUfSession;
        public static AutomatisationProcess theProgram;
        public static bool isDisposeCalled;
        private static string selectedGroup;

        public AutomatisationProcess()
        {
            try
            {
                theSession = Session.GetSession();
                theUI = UI.GetUI();
                theUfSession = UFSession.GetUFSession();
                isDisposeCalled = false;
            }
            catch (NXOpen.NXException e)
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, e.Message);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                theProgram = new AutomatisationProcess();

                Part workPart = theSession.Parts.Work;
                CAMSetup camSetup = workPart.CAMSetup;
                ListingWindow lw = theSession.ListingWindow;
                lw.Open();

                string parentGroupName = GetSelectedGroupFromNX(workPart, camSetup);

                if(parentGroupName == null) 
                {
                    return;
                }

                //string postDir = Environment.GetEnvironmentVariable("UGII_CAM_POST_DIR");
                string outputDirectory = Environment.GetEnvironmentVariable("UGII_RESULT_DIR");
                string postName = "MILL_3_AXIS";



                var postDialog = new PostprocessDialog(theSession, theUI);
                var response = postDialog.Launch();

                if (response == BlockDialog.DialogResponse.Cancel)
                {
                    return;
                }
                else if (response == BlockDialog.DialogResponse.Ok)
                {
                    postName = postDialog.GetSelectedPostprocessor();
                    outputDirectory = postDialog.GetOutputDirectory();
                }
                else
                {
                    lw.WriteLine("Неизвестный ответ диалога: " + response);
                    return;
                }

                //try
                //{
                //    Directory.CreateDirectory(outputDirectory);
                //    lw.WriteLine("Выходная директория создана: " + outputDirectory);
                //    LogToFile("Выходная директория создана: " + outputDirectory, lw);
                //}
                //catch (Exception ex)
                //{
                //    lw.WriteLine("Ошибка создания директории " + outputDirectory + " : " + ex.Message);
                //    LogToFile("Ошибка создания директории " + outputDirectory + " : " + ex.Message, lw);
                //    return;
                //}

                //string postSubDir = System.IO.Path.Combine(postDir, "mill_3_axis");
                //string postPath = System.IO.Path.Combine(postSubDir, postName + ".tcl");
                //string defPath = System.IO.Path.Combine(postSubDir, postName + ".def");
                //string puiPath = System.IO.Path.Combine(postSubDir, postName + ".pui");

                //lw.WriteLine("Поиск постпроцессора: " + postName);
                //lw.WriteLine("TCL путь: " + postPath);
                //lw.WriteLine("DEF путь: " + defPath);
                //lw.WriteLine("PUI путь: " + puiPath);

                //if (!File.Exists(postPath)) throw new FileNotFoundException("TCL файл не найден", postPath);
                //if (!File.Exists(defPath)) throw new FileNotFoundException("DEF файл не найден", defPath);
                //if (!File.Exists(puiPath)) lw.WriteLine("Предупреждение: PUI файл не найден");

                //string parentGroupName = args.Length > 0 ? args[0] : "U1";
                NCGroup parentGroup = FindParentGroup(camSetup, parentGroupName, lw);

                if (parentGroup == null)
                {
                    lw.WriteLine("Программа остановлена: не выбрана группа.");
                    return;
                }


                lw.WriteLine("Программы принадлежащие " + parentGroupName);
                
                var programData = CollectProgramData(camSetup, parentGroup, lw);

                //foreach (var program in programData)
                //{
                //    lw.WriteLine(program.Key + " - " + string.Join(", ", program.Value));
                //}

                ProcessPrograms(workPart, programData, outputDirectory, postName, lw);
            }
            catch (NXException e)
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, e.Message);
            }

        }

        private static string GetSelectedGroupFromNX(Part part, CAMSetup camSetup)
        {
            try
            {
                int selectedCount = theUI.SelectionManager.GetNumSelectedObjects();
                if (selectedCount == 0)
                {
                    theSession.ListingWindow.WriteLine("Нет выбранных объектов");
                    return null;
                }

                NCGroup foundGroup = null;
                int groupCount = 0;

                for (int i = 0; i < selectedCount; i++)
                {
                    try
                    {
                        TaggedObject selectedObject = theUI.SelectionManager.GetSelectedTaggedObject(i);

                        if (selectedObject is NCGroup ncGroup)
                        {
                            foundGroup = ncGroup;
                            groupCount++;
                        }
                    }
                    catch (Exception e)
                    {
                        theSession.ListingWindow.WriteLine($"Ошибка при обработке выбранного объекта {i}: {e.Message}");
                    }
                }

                if (groupCount == 0)
                {
                    theSession.ListingWindow.WriteLine("Среди выбранных объектов нет CAM групп");
                    return string.Empty;
                }
                else if(groupCount > 1)
                {
                    theSession.ListingWindow.WriteLine("Выбрано более одной CAM группы. Пожалуйста, выберите только одну группу.");
                    return string.Empty; ;
                }

                return foundGroup.Name;
            }
            catch (Exception e)
            {
                theSession.ListingWindow.WriteLine($"Ошибка получения группы: {e.Message}");
                return string.Empty;
            }
        }

        private static NCGroup FindParentGroup(CAMSetup camSetup, string parentGroupName, ListingWindow lw)
        {
            foreach (NCGroup group in camSetup.CAMGroupCollection)
            {
                if (group.Name == parentGroupName)
                {
                    return group;
                }
            }

            lw.WriteLine("Родительская группа " + parentGroupName + " не найдена");
            LogToFile("Родительская группа " + parentGroupName + " не найдена", lw);
            return null;
        }

        private static Dictionary<string, List<CAMObject>> CollectProgramData(CAMSetup camSetup, NCGroup parentGroup, ListingWindow lw)
        {
            var programData = new Dictionary<string, List<CAMObject>>();

            foreach (NCGroup group in camSetup.CAMGroupCollection)
            {
                if (group.GetParent() == parentGroup)
                {
                    var operations = new List<CAMObject>();

                    foreach (CAMObject member in group.GetMembers())
                    {
                        if (member is Operation)
                        {
                            //Operation operation = (Operation)member;
                            operations.Add(member);
                            theSession.CAMSession.PathDisplay.ShowToolPath(member);
                        }
                    }

                    if (operations.Count > 0)
                    {
                        programData.Add(group.Name, operations);
                        lw.WriteLine("Найдена программа: " + group.Name + " (" + operations.Count + " операций)");
                    }
                }
            }
            return programData;
        }

        private static void ProcessPrograms(Part workPart, Dictionary<string, List<CAMObject>> programData,
                                          string outputDirectory, string postName, ListingWindow lw)
        {
            foreach (var prog in programData)
            {
                string programName = prog.Key;
                string outputFile = System.IO.Path.Combine(outputDirectory, programName + ".mpf");
                CAMObject[] operations = prog.Value.ToArray();

                lw.WriteLine("\nОбработка программы: " + programName);

                try
                {
                    bool allComplete = true;
                    foreach (Operation op in operations)
                    {
                        if (op == null)
                        {
                            lw.WriteLine("Предупреждение: Обнаружена пустая операция");
                            LogToFile("Предупреждение: Обнаружена пустая операция", lw);
                            continue;
                        }

                        var status = op.GetStatus();
                        if (status == CAMObject.Status.Regen)
                        {
                            lw.WriteLine("Внимание: Операция " + op.Name + " не была успешно рассчитана(статус: " + status + ")");
                            LogToFile("Внимание: Операция " + op.Name + " не была успешно рассчитана(статус: " + status + ")", lw);
                            allComplete = false;
                        }
                    }

                    if (!allComplete)
                    {
                        lw.WriteLine("Пропуск программы: не все операции рассчитаны");
                        LogToFile("Пропуск программы: не все операции рассчитаны", lw);
                        continue;
                    }

                    lw.WriteLine("Запуск постпроцессинга с постпроцессором: " + postName);
                    LogToFile("Запуск постпроцессинга с постпроцессором: " + postName, lw);

                    foreach (var op in operations)
                    {

                        //workPart.CAMSetup.PostprocessWithPostModeSetting(operations, "MILL_3AXIS_SINUMERIK_840D_MM", "D:\\NC_PROGRAMS\\1" + prog.Key + ".ptp",
                        //                                                    CAMSetup.OutputUnits.PostDefined,
                        //                                                    CAMSetup.PostprocessSettingsOutputWarning.PostDefined,
                        //                                                    CAMSetup.PostprocessSettingsReviewTool.PostDefined,
                        //                                                    CAMSetup.PostprocessSettingsPostMode.Normal);

                        workPart.CAMSetup.PostprocessWithPostModeSetting(operations, postName, outputFile,
                                                                            CAMSetup.OutputUnits.PostDefined,
                                                                            CAMSetup.PostprocessSettingsOutputWarning.PostDefined,
                                                                            CAMSetup.PostprocessSettingsReviewTool.PostDefined,
                                                                            CAMSetup.PostprocessSettingsPostMode.Normal);

                        //LogToFile("Постпроцессинг закончен", lw);

                        if (File.Exists(outputFile))
                        {
                            var fileInfo = new FileInfo(outputFile);
                            lw.WriteLine("УП успешно сохранен в: " + outputFile);
                            lw.WriteLine("Размер файла: " + fileInfo.Length + " байт");
                        }
                        else
                        {
                            lw.WriteLine("Ошибка: Файл УП не был создан");
                        }
                    }
                }
                catch (Exception ex)
                {
                    lw.WriteLine("Ошибка обработки программы: " + programName + " : " + ex.Message);
                    LogToFile("Ошибка обработки программы: " + programName + " : " + ex.Message, lw);
                    lw.WriteLine("Подробности:");
                    lw.WriteLine(ex.StackTrace);
                    LogToFile("StackTrace:" + ex.StackTrace, lw);
                    LogToFile("Data:" + ex.Data.ToString(), lw);
                    LogToFile("GetBaseException:" + ex.GetBaseException(), lw);
                    LogToFile("HelpLink:" + ex.HelpLink, lw);
                    LogToFile("HResult:" + ex.HResult, lw);
                    LogToFile("Source:" + ex.Source, lw);
                    LogToFile("TargetSite:" + ex.TargetSite, lw);
                }
            }
        }

        private static void LogToFile(string message, ListingWindow lw)
        {
            string logPath = @"D:\NX_Logs\journal_log.txt";

            try
            {
                // Создаем директорию, если её нет
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logPath));

                // Записываем сообщение в файл
                using (StreamWriter sw = new StreamWriter(logPath, true))
                {
                    sw.WriteLine("[" + DateTime.Now + "]" + message);
                }
            }
            catch (Exception ex)
            {
                lw.WriteLine("Ошибка записи в лог-файл: " + ex.Message);
            }
        }

        public void Dispose()
        {
            try
            {
                if (isDisposeCalled == false)
                {

                }
                isDisposeCalled = true;
            }
            catch (NXOpen.NXException e)
            {
                UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, e.Message);
            }
        }

        public static int GetUnloadOption(string arg)
        {

            //Unloads the image explicitly, via an unload dialog
            //return System.Convert. ToInt32(Session.LibraryUnloadOption. Explicitly);

            //Unloads the image inmediately efter execution within NX
            return System.Convert.ToInt32(Session.LibraryUnloadOption.Immediately);

            //Unloads the image when the NX session terminates
            //return System.Convert.ToInt32(Session.LibraryUnloadOption.AtTermination);
        }
    }
}
