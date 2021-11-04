using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routing;


namespace Nop.Plugin.ExternalAuth.Auth0.Infrastructure
{
    public class RouteProvider
    {
        public void RegisterRoutes(IRouteBuilder routeBuilder)
        {
            routeBuilder.MapLocalizedRoute("AdminLogin", "/adminlogin", new { controller = "CustomAuthentication", action = "Login" });
        }

        public int Priority => 1;
    }
}
