using FireBaseAuthenticator.Model;

namespace FireBaseAuthenticator.KijijiHelperServices
{
    public interface IFireBaseSettingService
    {
        int GetVipAccountPostNumber();
        string GetHistoryUrl();
        string GetV3Url();
    }
}
