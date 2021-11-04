using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Nop.Core;
using Nop.Core.Http;
using Nop.Plugin.ExternalAuth.Auth0.Models;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.ExternalAuth.Auth0.Controllers
{
    public class CustomAuthenticationController : BasePluginController
    {
        #region Fields

        private readonly CustomAuthenticationSettings _customExternalAuthSettings;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly IOptionsMonitorCache<CookieOptions> _optionsCache;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor

        public CustomAuthenticationController(CustomAuthenticationSettings customExternalAuthSettings,
            IExternalAuthenticationService externalAuthenticationService,
            ILocalizationService localizationService,
            IOptionsMonitorCache<CookieOptions> optionsCache,
            IPermissionService permissionService,
            ISettingService settingService,
            INotificationService notificationService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IHttpContextAccessor httpContextAccessor)
        {
            _customExternalAuthSettings = customExternalAuthSettings;
            this._externalAuthenticationService = externalAuthenticationService;
            this._localizationService = localizationService;
            this._optionsCache = optionsCache;
            this._permissionService = permissionService;
            this._settingService = settingService;
            _notificationService = notificationService;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            var model = new ConfigurationModel
            {
                ClientId = _customExternalAuthSettings.ClientKeyIdentifier,
                ClientSecret = _customExternalAuthSettings.ClientSecret,
                AllowEmployeesNopLogin = _customExternalAuthSettings.AllowEmployeesNopLogin,
                Diagnostics = GetDiagnostics()
            };

            return View("~/Plugins/ExternalAuth.Auth0/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageExternalAuthenticationMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //save settings
            _customExternalAuthSettings.ClientKeyIdentifier = model.ClientId;
            _customExternalAuthSettings.ClientSecret = model.ClientSecret;
            _customExternalAuthSettings.AllowEmployeesNopLogin = model.AllowEmployeesNopLogin;
            _settingService.SaveSetting(_customExternalAuthSettings);

            //clear authentication options cache
            _optionsCache.TryRemove(CustomAuthenticationDefaults.AuthenticationScheme);

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        public IActionResult Login(string returnUrl = "/")
        {
            if (string.IsNullOrEmpty(_customExternalAuthSettings.ClientKeyIdentifier) || string.IsNullOrEmpty(_customExternalAuthSettings.ClientSecret))
                throw new NopException("Custom Authentication module not configured");

            //configure login callback action
            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LoginCallback", "CustomAuthentication", new { returnUrl = returnUrl })
            };
            authenticationProperties.SetString("ErrorCallback", Url.RouteUrl("Login", new { returnUrl }));
            return Challenge(authenticationProperties, CustomAuthenticationDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> LoginCallback(string code, string returnUrl = "/")
        {
            //authenticate employee user
            var authenticateResult = await this.HttpContext.AuthenticateAsync(CustomAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded || !authenticateResult.Principal.Claims.Any())
                return RedirectToRoute("Login");

            //create external authentication parameters
            var authenticationParameters = new ExternalAuthenticationParameters
            {
                ProviderSystemName = CustomAuthenticationDefaults.ProviderSystemName,
                AccessToken = await this.HttpContext.GetTokenAsync(CustomAuthenticationDefaults.AuthenticationScheme, "access_token"),
                Email = GetEmailClaim(authenticateResult),
                ExternalIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                ExternalDisplayIdentifier = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Name)?.Value,
                Claims = authenticateResult.Principal.Claims.Select(claim => new ExternalAuthenticationClaim(claim.Type, claim.Value)).ToList()
            };
            //authenticate Nop user
            return _externalAuthenticationService.Authenticate(authenticationParameters, returnUrl);
        }

        private static string GetEmailClaim(AuthenticateResult authenticateResult)
        {
            var email = authenticateResult.Principal.FindFirst(claim => claim.Type == ClaimTypes.Email)?.Value; //if it's not in the default location
            if (string.IsNullOrEmpty(email))
            {//then check a custom location
                email = authenticateResult.Principal.FindFirst(claim => claim.Type == "https://claims.auth0.com/email")?.Value;
                return email;
            }
            return email;
        }

        //May not be needed
        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CustomAuthenticationDefaults.AuthenticationScheme, new AuthenticationProperties { });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private Dictionary<string, string> GetDiagnostics()
        {
            var diags = new Dictionary<string, string>();
            var xFwdProtoHeader = "Missing";
            if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(NopHttpDefaults.HttpXForwardedProtoHeader))
            {
                xFwdProtoHeader = _httpContextAccessor.HttpContext.Request.Headers[NopHttpDefaults.HttpXForwardedProtoHeader];
            }
            diags.Add("Store Context Url", _storeContext.CurrentStore.Url);
            diags.Add("Conn Secured?", _webHelper.IsCurrentConnectionSecured().ToString());
            diags.Add("Web Helper Store Host", _webHelper.GetStoreHost(_webHelper.IsCurrentConnectionSecured()));
            diags.Add("Forwarded Proto", xFwdProtoHeader);
            diags.Add("---HEADERS---", "-----------------");
            diags.Add("-------------", "-----------------");
            foreach (var header in _httpContextAccessor.HttpContext.Request.Headers.Keys)
            {
                diags.Add(header, _httpContextAccessor.HttpContext.Request.Headers[header]);
            }
            return diags;
        }
        #endregion
    }
}
