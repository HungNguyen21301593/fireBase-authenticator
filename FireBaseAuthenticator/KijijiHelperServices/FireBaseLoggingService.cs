using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text;
using FireBaseAuthenticator.Exceptions;
using Newtonsoft.Json;

namespace FireBaseAuthenticator.KijijiHelperServices
{
    public class FireBaseLoggingService : IFireBaseLoggingService
    {
        private IMachineInformationService MachineInformationService { get; }
        private IFireBaseSettingService FireBaseSettingService { get; }

        public FireBaseLoggingService(IMachineInformationService machineInformationService, IFireBaseSettingService fireBaseSettingService)
        {
            MachineInformationService = machineInformationService ?? throw new ArgumentNullException(nameof(machineInformationService));
            FireBaseSettingService = fireBaseSettingService;
        }

        public async Task LogError(string message)
        {
            var infoMessage = $"Error - {message}";
            await LogHistory(infoMessage);
        }

        public async Task LogInfo(string message)
        {
            var infoMessage = $"Info - {message}";
            await LogHistory(infoMessage);
        }

        private async Task LogHistory(string message)
        {
            try
            {
                var machineName = MachineInformationService.GetMachineName();
                var userName = MachineInformationService.GetUserName();
                var formatMessage = $"[{DateTime.UtcNow}][D-{machineName}][U-{userName}]: {message}";
                var jsonString = JsonConvert.SerializeObject(formatMessage);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                using var httpClient = new HttpClient();
                await httpClient.PostAsync(FireBaseSettingService.GetHistoryUrl(), content);
            }
            catch (Exception)
            {
                throw new LogFailedException("Could not send the history");
            }
        }
    }
}
