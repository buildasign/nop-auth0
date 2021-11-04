using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;

namespace Nop.Plugin.ExternalAuth.Auth0.Services
{
    public class CustomExternalAuthenticationService : ExternalAuthenticationService
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IWorkContext _workContext;
        private readonly IIsEmployeeSpecification _isEmployeeSpecification;

        public CustomExternalAuthenticationService(CustomerSettings customerSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            IAuthenticationPluginManager authenticationPluginManager,
            IAuthenticationService authenticationService,
            ICustomerActivityService customerActivityService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IRepository<ExternalAuthenticationRecord> externalAuthenticationRecordRepository,
            IShoppingCartService shoppingCartService,
            IStoreContext storeContext,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            IIsEmployeeSpecification isEmployeeSpecification) : base(customerSettings, externalAuthenticationSettings, authenticationPluginManager, authenticationService, customerActivityService, customerRegistrationService, customerService, eventPublisher, genericAttributeService, localizationService, externalAuthenticationRecordRepository, shoppingCartService, storeContext, workContext, workflowMessageService, localizationSettings)
        {
            _authenticationService = authenticationService;
            _customerActivityService = customerActivityService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _shoppingCartService = shoppingCartService;
            _workContext = workContext;
            _isEmployeeSpecification = isEmployeeSpecification;
        }

        /// <summary>
        /// Login passed user
        /// </summary>
        /// <param name="user">User to login</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        protected override IActionResult LoginUser(Customer user, string returnUrl)
        {
            //migrate shopping cart
            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, user, true);

            var isPersistent = _isEmployeeSpecification.IsEmployee(user.Email);

            //authenticate
            _authenticationService.SignIn(user, isPersistent);

            //raise event       
            _eventPublisher.Publish(new CustomerLoggedinEvent(user));

            //activity log
            _customerActivityService.InsertActivity(user, "PublicStore.Login",
                _localizationService.GetResource("ActivityLog.PublicStore.Login"), user);

            return SuccessfulAuthentication(returnUrl);
        }
    }
}
