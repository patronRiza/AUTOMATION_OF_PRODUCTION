using AutomationOfPostprocessing.Services;
using AutomationOfPostprocessing.Services.CAM;
using AutomationOfPostprocessing.Services.FileSystem;
using AutomationOfPostprocessing.UI;
using AutomationOfPostprocessing.UI.Notifiers;
using AutomationOfPostprocessing.Utils;
using NXOpen;
using NXOpen.CAM;
using System;

namespace AutomationOfPostprocessing
{
    public class AutomatisationProcess
    {
        public static AutomatisationProcess _theProgram;
        //private static Session _session;
        //private static NXOpen.UI _ui;
        //private static IUserNotifier _notifier;
        //private static NXLogger _logger;
        //private static ParentGroupService _parentGroupService;
        //private static ProgramService _programService;
        //private static PostprocessConfigurator _postprocessConfigurator;
        private static ComponentInitializer _initializer;
        private static DataValidator _validator;
        public static bool isDisposeCalled;
     

        public AutomatisationProcess()
        {
            try
            {
                //_session = Session.GetSession();
                //_ui = NXOpen.UI.GetUI();
                //_notifier = new NXUserNotifier(_ui);
                //_logger = new NXLogger(_notifier);
                //_parentGroupService = new ParentGroupService(_session, _logger, _ui);
                //_programService = new ProgramService(_session, _logger);
                //_postprocessConfigurator = new PostprocessConfigurator(_ui, _logger, _session);
                _initializer = new ComponentInitializer();
                _validator = new DataValidator(_initializer.Logger);
            }
            catch (Exception ex)
            {
                _initializer.Logger.LogError(ex);
            }
        }

        public static void Main(string[] args)
        {
            try
            {
                AutomatisationProcess.Run();
            }
            catch(Exception ex)
            {
                _initializer.Logger.LogError(ex);
            }
        }

        private static void Run()
        {
            try
            {
                _theProgram = new AutomatisationProcess();

                var selectedParentGroup = _initializer.ParentGroupService.GetSelectGroup();
                _validator.ValidateParentGroup(selectedParentGroup);

                var (success, postName, extention, outputDir) = _initializer.PostprocessConfigurator.Configure();
                _validator.ValidatePostprocessConfigurator(success);

                var programData = _initializer.ProgramService.CollectProgramData(selectedParentGroup);

                _initializer.ProgramService.ProcessPrograms(programData, outputDir, postName, extention);
            }
            catch (Exception ex)
            {
                _initializer.Logger.LogError(ex);
                throw new ApplicationException("Ошибка выполнения процесса автоматизации", ex);
            }
            finally
            {
                _theProgram = null;
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
