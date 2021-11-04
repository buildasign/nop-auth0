using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;

namespace Nop.Plugin.ExternalAuth.Auth0.Services
{
    public class CustomCustomerRegistrationService : CustomerRegistrationService
    {
        private readonly IIsEmployeeSpecification _isEmployeeSpecification;

        public CustomCustomerRegistrationService(CustomerSettings customerSettings,
            ICustomerService customerService,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IRewardPointService rewardPointService,
            IStoreService storeService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            RewardPointsSettings rewardPointsSettings,
            IIsEmployeeSpecification isEmployeeSpecification) : base(customerSettings, customerService, encryptionService, eventPublisher, genericAttributeService, localizationService, newsLetterSubscriptionService, rewardPointService, storeService, workContext, workflowMessageService, rewardPointsSettings)
        {
            _isEmployeeSpecification = isEmployeeSpecification;
        }
        public override CustomerLoginResults ValidateCustomer(string usernameOrEmail, string password)
        {
            var settings = EngineContext.Current.Resolve<CustomAuthenticationSettings>();

            if (!settings.AllowEmployeesNopLogin && _isEmployeeSpecification.IsEmployee(usernameOrEmail))
            {
                return CustomerLoginResults.NotActive;
            }
            return base.ValidateCustomer(usernameOrEmail, password);
        }
    }
}
