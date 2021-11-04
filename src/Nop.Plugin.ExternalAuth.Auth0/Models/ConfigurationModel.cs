using System.Collections.Generic;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;


namespace Nop.Plugin.ExternalAuth.Auth0.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        [NopResourceDisplayName("Plugins.ExternalAuth.Auth0.ClientKeyIdentifier")]
        public string ClientId { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Auth0.ClientSecret")]
        public string ClientSecret { get; set; }

        [NopResourceDisplayName("Plugins.ExternalAuth.Auth0.AllowEmployeesNopLogin")]
        public bool AllowEmployeesNopLogin { get; set; }
        public Dictionary<string, string> Diagnostics { get; set; }
    }
}
