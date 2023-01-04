using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzFunctionCheckAvailability.Authentication
{
    public class AuthenticationConfig
    {
        /// <summary>
        /// instance of Azure AD, for example public Azure or a Sovereign cloud (Azure China, Germany, US government, etc ...)
        /// </summary>
        public string Instance { get; set; } = $"https://login.microsoftonline.com/{0}";

        /// <summary>
        /// The Tenant is:
        /// - either the tenant ID of the Azure AD tenant in which this application is registered (a guid)
        /// or a domain name associated with the tenant
        /// - or 'organizations' (for a multi-tenant application)
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Guid used by the application to uniquely identify itself to Azure AD
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// URL of the authority for the downstream api.
        /// </summary>
        public string DownStreamAuthority { get; set; }

        /// <summary>
        /// Client secret (application password)
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// The description of the certificate to be used to authenticate your application.
        /// </summary>
        /// <remarks>Daemon applications can authenticate with AAD through two mechanisms: ClientSecret
        /// or a certificate previously shared with AzureAD during the application registration 
        /// (and identified by this CertificateDescription)
        /// <remarks> 
        public CertificateDescription Certificate { get; set; }

        /// <summary>
        /// Web Api base URL
        /// </summary>
        public string DownstreamBaseAddress { get; set; }

        /// <summary>
        /// Web Api scope. With client credentials flows, the scopes is ALWAYS of the shape "resource/.default"
        /// </summary>
        public string DownstreamScopes { get; set; }


    }
}
