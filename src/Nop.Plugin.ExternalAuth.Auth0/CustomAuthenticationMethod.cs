using Nop.Core;
using Nop.Services.Authentication.External;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.ExternalAuth.Auth0
{
    /// <summary>
    ///  Represents method for the authentication with the Auth0 account
    /// </summary>
    public class CustomAuthenticationMethod : BasePlugin, IExternalAuthenticationMethod
    {
        #region Fields

        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public CustomAuthenticationMethod(IWebHelper webHelper,
            ILocalizationService localizationService)
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/CustomAuthentication/Configure";
        }

        public string GetPublicViewComponentName()
        {
            return "CustomAuthentication";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //locales
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientKeyIdentifier", "App ID/API Key");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientKeyIdentifier.Hint", "Enter your app ID/API key here. You can find it on your Auth0 application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientSecret", "App Secret");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientSecret.Hint", "Enter your app secret here. You can find it on your Auth0 application page.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Auth0.AllowEmployeesNopLogin", "Allow NOP Login for Employees.");
            _localizationService.AddOrUpdatePluginLocaleResource("Plugins.ExternalAuth.Auth0.AllowEmployeesNopLogin.Hint", "Allow Employees to sign in via NOP in addition to Auth0.");

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //locales
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientKeyIdentifier");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientKeyIdentifier.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientSecret");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Auth0.ClientSecret.Hint");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Auth0.AllowEmployeesNopLogin");
            _localizationService.DeletePluginLocaleResource("Plugins.ExternalAuth.Auth0.AllowEmployeesNopLogin.Hint");

            base.Uninstall();
        }

        #endregion
    }
}
