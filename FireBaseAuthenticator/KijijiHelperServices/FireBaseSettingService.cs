namespace FireBaseAuthenticator.KijijiHelperServices
{
    public class FireBaseSettingService: IFireBaseSettingService
    {
        public int VipAccountPostNumber = 99999;
        public string V3Url = "https://subscriptions-e1198-default-rtdb.firebaseio.com/v3/";
        public string HistoryUrl = "https://subscriptions-e1198-default-rtdb.firebaseio.com/history.json";

        public int GetVipAccountPostNumber()
        {
            return VipAccountPostNumber;
        }

        public string GetHistoryUrl()
        {
            return HistoryUrl;
        }

        public string GetV3Url()
        {
            return V3Url;
        }

    }
}