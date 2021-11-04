using System;

namespace Nop.Plugin.ExternalAuth.Auth0.Services
{
    public interface IIsEmployeeSpecification
    {
        bool IsEmployee(string usernameOrEmail);
    }

    public class IsEmployeeSpecification : IIsEmployeeSpecification
    {
        public bool IsEmployee(string usernameOrEmail)
        {
            return !string.IsNullOrEmpty(usernameOrEmail)
                   && usernameOrEmail.EndsWith("@<yourdomain>.com", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
