using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;

namespace AzFunctionCheckAvailability.Authentication
{
    public static class AuthorizedClientExtensions
    {

        public static void AddAuthorizedHttpClient(this IServiceCollection services, string httpClientName)
        {
            services.AddSingleton(container =>
            {
                var config = container.GetRequiredService<IConfiguration>();
                return new ConfidentialClient(config);
            });
            services.AddTransient<AuthHeaderHandler>();
            services.AddHttpClient(httpClientName).AddHttpMessageHandler<AuthHeaderHandler>();
        }

        public static void AddAuthorizedHttpClient(this IServiceCollection services, string httpClientName, string baseurl = "")
        {
            services.AddSingleton(container =>
            {
                var config = container.GetRequiredService<IConfiguration>();
                return new ConfidentialClient(config);
            });
            services.AddTransient<AuthHeaderHandler>();
            services.AddHttpClient(httpClientName, client => { client.BaseAddress = new Uri(baseurl); }).AddHttpMessageHandler<AuthHeaderHandler>();
        }
    }
}
