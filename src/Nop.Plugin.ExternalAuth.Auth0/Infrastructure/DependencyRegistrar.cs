using Autofac;
using Nop.Plugin.ExternalAuth.Auth0.Services;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Services.Authentication.External;
using Nop.Services.Customers;

namespace Nop.Plugin.ExternalAuth.Auth0.Infrastructure
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 101;

        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<IsEmployeeSpecification>().As<IIsEmployeeSpecification>().InstancePerLifetimeScope();
            builder.RegisterType<CustomCustomerRegistrationService>().As<ICustomerRegistrationService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomExternalAuthenticationService>().As<IExternalAuthenticationService>().InstancePerLifetimeScope();
        }
    }
}
