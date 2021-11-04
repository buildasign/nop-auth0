using Nop.Core.Configuration;


namespace Nop.Plugin.ExternalAuth.Auth0
{
    /// <summary>
    /// Represents settings of the custom authentication method
    /// </summary>
    public class CustomAuthenticationSettings : ISettings
    {
        /// <summary>
        /// Gets or sets OAuth2 client identifier
        /// </summary>
        public string ClientKeyIdentifier { get; set; }

        /// <summary>
        /// Gets or sets OAuth2 client secret
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// If enabled allows employee accounts to sign on via NOP.
        /// If disabled employee accounts must sign in via Auth0.
        /// </summary>
        public bool AllowEmployeesNopLogin { get; set; }
    }
}
