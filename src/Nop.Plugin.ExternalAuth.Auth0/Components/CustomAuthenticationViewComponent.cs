using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;


namespace Nop.Plugin.ExternalAuth.Auth0.Components
{
    [ViewComponent(Name = "CustomAuthentication")]
    public class CustomAuthenticationViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/ExternalAuth.Auth0/Views/PublicInfo.cshtml");
        }
    }
}
