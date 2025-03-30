using AutomationOfPostprocessing.Services.CAM;
using AutomationOfPostprocessing.UI;
using AutomationOfPostprocessing.UI.Notifiers;
using NXOpen;
using System;

namespace AutomationOfPostprocessing.Services
{
    public class ComponentInitializer
    {
        public Session Session { get; }
        public NXOpen.UI UI { get; }
        public IUserNotifier Notifier { get; }
        public NXLogger Logger { get; }
        public ParentGroupService ParentGroupService { get; }
        public ProgramService ProgramService { get; }
        public PostprocessConfigurator PostprocessConfigurator { get; }

        public ComponentInitializer()
        {
            try
            {
                Session = Session.GetSession();
                UI = NXOpen.UI.GetUI();
                Notifier = new NXUserNotifier(UI);
                Logger = new NXLogger(Notifier);
                ParentGroupService = new ParentGroupService(Session, Logger, UI);
                ProgramService = new ProgramService(Session, Logger);
                PostprocessConfigurator = new PostprocessConfigurator(UI, Logger, Session);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
        }
    }
}
