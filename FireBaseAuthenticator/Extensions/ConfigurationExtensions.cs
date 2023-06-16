using FireBaseAuthenticator.KijijiHelperServices;
using Microsoft.Extensions.DependencyInjection;

namespace FireBaseAuthenticator.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddFireBaseRegistrationDependencies(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDeviceRegistrationService, DeviceRegistrationService>();
            serviceCollection.AddTransient<IMachineInformationService, MachineInformationService>();
            serviceCollection.AddTransient<IFireBaseLoggingService, FireBaseLoggingService>();
            serviceCollection.AddTransient<IFireBaseSettingService, FireBaseSettingService>();
            return serviceCollection;
        }
    }
}
