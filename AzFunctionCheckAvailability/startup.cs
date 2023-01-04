using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AzFunctionCheckAvailability.Authentication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Fintercept.SwitchCore.Messenger.Acquirer.Startup))]
namespace Fintercept.SwitchCore.Messenger.Acquirer
{
    public class Startup : FunctionsStartup
    {
        //optional settings load
        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            FunctionsHostBuilderContext context = builder.GetContext();

            builder.ConfigurationBuilder
            //.AddJsonFile(Path.Combine(context.ApplicationRootPath, $"appsettings.{context.EnvironmentName}.json"), optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();
        }

        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddOptions<FunctionSettings>().Configure<IConfiguration>((settings, configuration) =>
            //{
            //    configuration.GetSection("endpoints").Bind(settings);
            //});
            builder.Services.AddAuthorizedHttpClient("AuthHttpClient");

        }
    }
}