using AutomationOfPostprocessing.Services.CAM;
using AutomationOfPostprocessing.Suppliers;
using AutomationOfPostprocessing.UI;
using AutomationOfPostprocessing.UI.Notifiers;
using NXOpen;
using NXOpen.CAM;
using System;

namespace AutomationOfPostprocessing
{
    public class AutomatisationProcess
    {
        public static AutomatisationProcess _theProgram;
        private static Session _session;
        private static NXOpen.UI _ui;
        private static IUserNotifier _notifier;
        private static NXLogger _logger;
        private static ParentGroupSupplier _parentGroupSupplier;
        private static PostprocessConfigurator _postprocessConfigurator;
        private static ProgramsSupplier _programsSupplier;
        private static ProgramProcessor _programProcessor;
        public static bool isDisposeCalled;
     

        public AutomatisationProcess()
        {
            try
            {
                _session = Session.GetSession();
                _ui = NXOpen.UI.GetUI();
                _notifier = new NXUserNotifier(_ui);
                _logger = new NXLogger(_notifier);
                _parentGroupSupplier = new ParentGroupSupplier(_session, _logger, _ui);
                _postprocessConfigurator = new PostprocessConfigurator(_ui, _logger, _session);
                _programsSupplier = new ProgramsSupplier(_session);
                _programProcessor = new ProgramProcessor(_session, _logger);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
        }

        public static void Main(string[] args)
        {
            AutomatisationProcess.Run();
        }

        private static void Run()
        {
            try
            {
                _theProgram = new AutomatisationProcess();

                var selectedParentGroup = _parentGroupSupplier.GetSelectGroup();
                ValidateParentGroup(selectedParentGroup);

                var (success, postName, outputDir) = _postprocessConfigurator.Configure();
                ValidatePostprocessConfigurator(success);

                var programData = _programsSupplier.CollectProgramData(selectedParentGroup);

                _programProcessor.ProcessPrograms(programData, outputDir, postName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
            }
            finally
            {
                _theProgram = null;
            }
        }

        private static void ValidateParentGroup(NCGroup workPart)
        {
            if (workPart == null)
                throw new InvalidOperationException("Нет открытой рабочей части");
        }

        private static void ValidatePostprocessConfigurator(bool success)
        {
            if (!success)
                throw new InvalidOperationException("Нет доступных программ");
        }

        //public static void Main(string[] args)
        //{
        //    try
        //    {
        //        theProgram = new AutomatisationProcess();

        //        Part workPart = theSession.Parts.Work;
        //        CAMSetup camSetup = workPart.CAMSetup;
        //        ListingWindow lw = theSession.ListingWindow;
        //        lw.Open();

        //        string parentGroupName = GetSelectedGroupFromNX(workPart, camSetup);

        //        if(parentGroupName == null) 
        //        {
        //            return;
        //        }

        //        //string postDir = Environment.GetEnvironmentVariable("UGII_CAM_POST_DIR");
        //        string outputDirectory = Environment.GetEnvironmentVariable("UGII_RESULT_DIR");
        //        string postName = "MILL_3_AXIS";



        //        var postDialog = new PostprocessDialog(theSession, theUI);
        //        var response = postDialog.Launch();

        //        if (response == BlockDialog.DialogResponse.Cancel)
        //        {
        //            return;
        //        }
        //        else if (response == BlockDialog.DialogResponse.Ok)
        //        {
        //            postName = postDialog.GetSelectedPostprocessor();
        //            outputDirectory = postDialog.GetOutputDirectory();
        //        }
        //        else
        //        {
        //            lw.WriteLine("Неизвестный ответ диалога: " + response);
        //            return;
        //        }

        //        NCGroup parentGroup = FindParentGroup(camSetup, parentGroupName, lw);

        //        if (parentGroup == null)
        //        {
        //            lw.WriteLine("Программа остановлена: не выбрана группа.");
        //            return;
        //        }


        //        lw.WriteLine("Программы принадлежащие " + parentGroupName);

        //        var programData = CollectProgramData(camSetup, parentGroup, lw);

        //        //foreach (var program in programData)
        //        //{
        //        //    lw.WriteLine(program.Key + " - " + string.Join(", ", program.Value));
        //        //}

        //        ProcessPrograms(workPart, programData, outputDirectory, postName, lw);
        //    }
        //    catch (NXException e)
        //    {
        //        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, e.Message);
        //    }

        //}

        //public void Dispose()
        //{
        //    try
        //    {
        //        if (isDisposeCalled == false)
        //        {

        //        }
        //        isDisposeCalled = true;
        //    }
        //    catch (NXOpen.NXException e)
        //    {
        //        UI.GetUI().NXMessageBox.Show("Message", NXMessageBox.DialogType.Error, e.Message);
        //    }
        //}

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
