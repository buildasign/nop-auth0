using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Http;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.ExternalAuth.Auth0.Infrastructure
{
    public class CustomAuthenticationStartup : INopStartup
    {
        public void Configure(IApplicationBuilder application)
        {
            //Documentation: https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-2.2#when-it-isnt-possible-to-add-forwarded-headers-and-all-requests-are-secure-1
            application.Use((context, next) =>
            {
                if (context.Request.Headers.ContainsKey(NopHttpDefaults.HttpXForwardedProtoHeader))
                {//set scheme to https so OIDC will generate https returnUri for /signin-oidc
                    context.Request.Scheme = "https";
                }
                return next();
            });
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {//OpenIdConnect was added in registrar
        }

        public int Order => 1;
    }
}
