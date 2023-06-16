using System.Data;
using FireBaseAuthenticator.Exceptions;
using FireBaseAuthenticator.Extensions;
using FireBaseAuthenticator.Model;
using Newtonsoft.Json;

namespace FireBaseAuthenticator.KijijiHelperServices
{
    public class DeviceRegistrationService : IDeviceRegistrationService
    {
        private IMachineInformationService MachineInformationService { get; }
        private IFireBaseSettingService FireBaseSettingService { get; }
        private IFireBaseLoggingService FireBaseLoggingService { get; }

        public DeviceRegistrationService(IMachineInformationService machineInformationService,
            IFireBaseSettingService fireBaseSettingService,
            IFireBaseLoggingService fireBaseLoggingService)
        {
            MachineInformationService = machineInformationService ?? throw new ArgumentNullException(nameof(machineInformationService));
            FireBaseSettingService = fireBaseSettingService ?? throw new ArgumentNullException(nameof(fireBaseSettingService));
            FireBaseLoggingService = fireBaseLoggingService ?? throw new ArgumentNullException(nameof(fireBaseLoggingService));
        }

        public async Task VerifyDevice()
        {
            try
            {
                var deviceName = MachineInformationService.GetMachineName();
                var deviceInfo = await GetDeviceInformation(deviceName);
                if (deviceInfo.IsVipDevice(FireBaseSettingService.GetVipAccountPostNumber()))
                {
                    await FireBaseLoggingService.LogInfo("Device is Vip, so verified");
                    return;
                }

                VerifyIfDeviceIsNotExpired(deviceInfo);
                VerifyIfRePostLimitIsValid(deviceInfo);
                await FireBaseLoggingService.LogInfo($"Device is verified, DeviceIsNotExpired + RePostLimitIsValid");
            }
            catch (GetDeviceInfoException)
            {
                await FireBaseLoggingService.LogError($"Could not get device's info, {typeof(GetDeviceInfoException)}");
                throw;
            }
            catch (RePostLimitExceedException)
            {
                await FireBaseLoggingService.LogError($"Device is NOT verified, {typeof(RePostLimitExceedException)}");
                throw;
            }
            catch (DeviceIsExpiredException)
            {
                await FireBaseLoggingService.LogError($"Device is NOT verified, {typeof(DeviceIsExpiredException)}");
                throw;
            }
            catch (Exception)
            {
                await FireBaseLoggingService.LogError($"Unhandled exceptions");
                throw;
            }
        }

        public async Task<V3> UpdateNumberOfAllowAds(int? remainingPost = null)
        {
            try
            {
                var deviceName = MachineInformationService.GetMachineName();
                var deviceInfo = await GetDeviceInformation(deviceName);
                if (deviceInfo.RemainingPostLimit == FireBaseSettingService.GetVipAccountPostNumber())
                {
                    return deviceInfo;
                }
                var newRemainingPost = remainingPost ?? deviceInfo.RemainingPostLimit - 1;
                deviceInfo.RemainingPostLimit = newRemainingPost;
                using var httpClient = new HttpClient();
                await httpClient.PutAsync(
                    $"{FireBaseSettingService.GetV3Url()}/{deviceName}.json", new StringContent(JsonConvert.SerializeObject(deviceInfo)));
                await FireBaseLoggingService.LogInfo($"remainingPost is updated: {newRemainingPost}");
                return deviceInfo;
            }
            catch (Exception e)
            {
                await FireBaseLoggingService.LogError($"failed to update remainingPost: {remainingPost}");
                Console.WriteLine(e);
                throw;
            }
        }

        private void VerifyIfDeviceIsNotExpired(V3 deviceInfo)
        {
            if (deviceInfo.IsNotExpired())
            {
                return;
            }
            var errorMessage = "PC is expired";
            throw new DeviceIsExpiredException(errorMessage);
        }

        private void VerifyIfRePostLimitIsValid(V3 deviceInfo)
        {
            if (deviceInfo.IsRePostLimitIsValid())
            {
                return;
            }
            var errorMessage =
                $"I'm sorry, but you have reached the limit of allowed posts." +
                $"If you'd like to make more posts, please contact the developer to request an extension. " +
                $"If you make a payment via PayPal at paypal.me/hunghung1404, you will receive a {5}% discount." +
                $"So 20 more posts for just 5 USD. Thank you for using my application";
            throw new RePostLimitExceedException(errorMessage);
        }

        public async Task<V3> GetDeviceInformation(string? userPcName = null)
        {
            try
            {
                userPcName ??= MachineInformationService.GetMachineName();
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync($"{FireBaseSettingService.GetV3Url()}/{userPcName}.json");
                var result = await response.Content.ReadAsStringAsync();
                var settings = new JsonSerializerSettings()
                {
                    DateFormatString = "yyyy-MM-dd",
                    DateTimeZoneHandling = DateTimeZoneHandling.Local
                };
                var v3 = JsonConvert.DeserializeObject<V3>(result, settings) ?? throw new NoNullAllowedException(); ;
                return v3;
            }
            catch (Exception e)
            {
                await FireBaseLoggingService.LogError($"Failed to get device info for {userPcName}");
                throw new GetDeviceInfoException($"Could not get device info from Fire Base, device name: {userPcName}", e);
            }
        }
    }
}
