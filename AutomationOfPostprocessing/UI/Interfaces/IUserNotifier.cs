namespace AutomationOfPostprocessing.UI
{
    public interface IUserNotifier
    {
        void ShowInfo(string message);
        void ShowWarning(string message);
        void ShowError(string message);
    }
}
