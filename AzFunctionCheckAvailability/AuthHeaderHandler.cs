using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzFunctionCheckAvailability
{
    public class AuthHeaderHandler : DelegatingHandler
    {

        private ConfidentialClient _app;
        public AuthHeaderHandler(ConfidentialClient app) => _app = app;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var defaultRequestHeaders = request.Headers;
            if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
            {
                defaultRequestHeaders.Add("Accept", "application/json");
            }

            try
            {
                //MSAL maintains a token cache for reuse and handles refresh.
                var token = await _app.GetAccessToken();
                if (!string.IsNullOrEmpty(token))
                {
                    defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
            }
            catch (MsalUiRequiredException ex)
            {
                //result = await _app.AcquireToken(scopes).WithClaims(ex.Claims).ExecuteAsync()
            }


            return await base.SendAsync(request, cancellationToken);
        }
    }
}
