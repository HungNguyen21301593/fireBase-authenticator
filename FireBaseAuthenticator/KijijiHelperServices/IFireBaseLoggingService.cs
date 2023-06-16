namespace FireBaseAuthenticator.KijijiHelperServices
{
    public interface IFireBaseLoggingService
    {
        Task LogError(string message);
        Task LogInfo(string message);
    }
}
