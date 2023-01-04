using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace AzFunctionCheckAvailability
{
    public class Function1
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public Function1(IConfiguration configuration, IHttpClientFactory httpClient)
        {
            _configuration = configuration;
            _httpClientFactory = httpClient;
        }

        private static TelemetryClient telemetryClient;
        public async static Task RunAvailabilityTestAsync(HttpClient httpClient,string ep,ILogger log)
        {

            httpClient.BaseAddress = new Uri(ep);
            await httpClient.GetStringAsync(ep);
        }

        [FunctionName("FunctionCheckApp1")]
        public async Task RunAsync([TimerTrigger("*/15 * * * * *")] TimerInfo myTimer, ILogger log, ExecutionContext executionContext)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            //_array = Config.GetSection("array").Get<ArrayExample>();
            
            if (telemetryClient == null)
            {
                // Initializing a telemetry configuration for Application Insights based on connection string 

                var telemetryConfiguration = new TelemetryConfiguration();
                telemetryConfiguration.ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING");
                telemetryConfiguration.TelemetryChannel = new InMemoryChannel();
                telemetryClient = new TelemetryClient(telemetryConfiguration);
            }
            
            string location = Environment.GetEnvironmentVariable("REGION_NAME");
            
            //todo extract endpoints from config
            // Read configuration data
            //string keyName = "endpoints:entries";
            //var message = _configuration.GetSection(keyName);
            //var endpoints = _configuration.GetSection(keyName).Get<FunctionSettings>();
            //_array = _configuration.GetSection("array").Get<ArrayExample>();

            List<string> endpoints = new List<string>();
            endpoints.Add("https://www.google.com");
            

            foreach (var ep in endpoints)
            {
                //string testName = executionContext.FunctionName;
                string testName = ep;
                var availability = new AvailabilityTelemetry
                {
                    Name = testName,
                    RunLocation = location,
                    Success = false,
                };

                availability.Context.Operation.ParentId = Activity.Current.SpanId.ToString();
                availability.Context.Operation.Id = Activity.Current.RootId;
                var stopwatch = new Stopwatch();

                stopwatch.Start();

                try
                {
                    using (var activity = new Activity("AvailabilityContext"))
                    {
                        activity.Start();
                        availability.Id = Activity.Current.SpanId.ToString();
                        // Run business logic 
                        var _httpClient = _httpClientFactory.CreateClient("AuthHttpClient");
                        await RunAvailabilityTestAsync(_httpClient, ep, log);
                    }
                    availability.Success = true;
                }

                catch (Exception ex)
                {
                    availability.Message = ex.Message;
                    throw;
                }

                finally
                {
                    stopwatch.Stop();
                    availability.Duration = stopwatch.Elapsed;
                    availability.Timestamp = DateTimeOffset.UtcNow;
                    telemetryClient.TrackAvailability(availability);
                    telemetryClient.Flush();
                }
            }
        }
    }
}
