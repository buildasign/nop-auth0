namespace Nop.Plugin.ExternalAuth.Auth0
{
    /// <summary>
    /// Default values used by the custom authentication middleware
    /// </summary>
    public class CustomAuthenticationDefaults
    {
        /// <summary>
        /// System name of the external authentication method
        /// </summary>
        public const string ProviderSystemName = "ExternalAuth.Auth0";

        public const string AuthenticationScheme = "Auth0";
        public const string Domain = "https://<yourdomain>.auth0.com";
    }
}
