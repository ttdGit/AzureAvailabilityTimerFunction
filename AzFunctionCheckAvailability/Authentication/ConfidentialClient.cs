using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFunctionCheckAvailability.Authentication
{
    public class ConfidentialClient
    {


        public IConfiguration Configuration { get; }

        public IConfidentialClientApplication ClientApp { get; }

        private string[] Scopes;

        public ConfidentialClient(IConfiguration configuration)
        {
            Configuration = configuration;
            ClientApp = ClientApp == null ? CreateConfidentialClientApp(configuration) : ClientApp;
            SetConfiguration();
        }

        private void SetConfiguration()
        {
            AuthenticationConfig authconfig = GetADConfiguration();
            Scopes = new string[] { authconfig.DownstreamScopes };
        }

        private AuthenticationConfig GetADConfiguration()
        {
            var adSection = Configuration.GetSection("AzureAd");
            var authconfig = adSection.Get<AuthenticationConfig>();
            return authconfig;
        }

        public async Task<string> GetAccessToken()
        {
            // client credentials flows scope is the shape "resource/.default"
            // application permissions need to be set statically by a tenant administrator            
            AuthenticationResult result = null;
            result = await ClientApp.AcquireTokenForClient(Scopes).ExecuteAsync();
            return result.AccessToken;
        }
        private IConfidentialClientApplication CreateConfidentialClientApp(IConfiguration configuration)
        {
            AuthenticationConfig config = GetADConfiguration();

            var isUsingClientSecret = !string.IsNullOrWhiteSpace(config.ClientSecret) ? true : config.Certificate != null ? false : throw new Exception("You must choose between using client secret or certificate. Please update appsettings.json file.");

            IConfidentialClientApplication app;

            if (isUsingClientSecret)
            {
                app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithClientSecret(config.ClientSecret)
                    .WithAuthority(new Uri(config.DownStreamAuthority))
                    //.WithAuthority(new Uri("https://login.microsoftonline.com/ed00e63d-c799-4dd7-9286-a6714ad5b330/oauth2/v2.0/authorize"))
                    .Build();
            }

            else
            {
                ICertificateLoader certificateLoader = new DefaultCertificateLoader();
                certificateLoader.LoadIfNeeded(config.Certificate);

                app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                    .WithCertificate(config.Certificate.Certificate)
                    .WithAuthority(new Uri(config.DownStreamAuthority))
                    .Build();
            }

            app.AddInMemoryTokenCache();
            return app;

        }
    }
}
