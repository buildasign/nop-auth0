using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Authentication.External;

namespace Nop.Plugin.ExternalAuth.Auth0.Infrastructure
{
    /// <summary>
    /// Registration of custom authentication service (plugin)
    /// </summary>
    public class CustomAuthenticationRegistrar : IExternalAuthenticationRegistrar
    {
        /// <summary>
        /// Configure
        /// </summary>
        /// <param name="builder">Authentication builder</param>
        public void Configure(AuthenticationBuilder builder)
        {
            builder.AddOpenIdConnect(CustomAuthenticationDefaults.AuthenticationScheme, options =>
            {
                var settings = EngineContext.Current.Resolve<CustomAuthenticationSettings>();
                options.ClientId = settings.ClientKeyIdentifier;
                options.ClientSecret = settings.ClientSecret;
                options.Authority = CustomAuthenticationDefaults.Domain;
                options.ResponseType = "code";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.CallbackPath = new PathString("/signin-oidc");
                options.ClaimsIssuer = CustomAuthenticationDefaults.AuthenticationScheme;
                options.GetClaimsFromUserInfoEndpoint = true;
                options.SaveTokens = true;
                options.Events = new OpenIdConnectEvents
                {
                    //handle the logout redirection
                    OnRedirectToIdentityProviderForSignOut = (context) =>
                    {
                        var logoutUri =
                            $"https://<yourdomain>.auth0.com/v2/logout?client_id={settings.ClientKeyIdentifier}";
                        var postLogoutUri = context.Properties.RedirectUri;
                        if (!string.IsNullOrEmpty(postLogoutUri))
                        {
                            if (postLogoutUri.StartsWith("/"))
                            {
                                var request = context.Request;
                                postLogoutUri =
                                    $"{request.Scheme}://{request.Host}{request.PathBase}{postLogoutUri}";
                            }

                            logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                        }
                        context.Response.Redirect(logoutUri);
                        context.HandleResponse();
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
