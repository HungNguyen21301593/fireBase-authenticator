// See https://aka.ms/new-console-template for more information

using FireBaseAuthenticator.Extensions;
using FireBaseAuthenticator.KijijiHelperServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddFireBaseAuthenticatorDependencies();
    })
    .Build();
try
{
    await host.Services.GetRequiredService<IAuthenticatorService>().VerifyDevice();
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

await host.RunAsync();