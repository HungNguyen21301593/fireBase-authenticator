using FireBaseAuthenticator.Model;

namespace FireBaseAuthenticator.KijijiHelperServices
{
    public interface IDeviceRegistrationService
    {
        Task VerifyDevice();
        Task<V3> UpdateNumberOfAllowAds(int? remainingPost = null);
        Task<V3> GetDeviceInformation(string? userPcName = null);
    }
}
