# nop-auth0
A NopCommerce plugin allowing authentication via Auth0/OpenIdConnect to Active Directory domain.

#How to use
1. Customize your code using the options in the section "How to Customize" below. This will not work out of the box.
2. Build and deploy the plugin.
3. Install the plugin in NopCommerce
4. Go to the plugin's configuration screen to enter your client ID and secret.
5. Restart NopCommerce.
6. To login with Auth0, access the endpoint /adminlogin from one of the site's URL's.
7. Regular customers can still log in via /login as normal. If you wish to change this behavior and make Auth0 the only way to login, then see the Facebook plugin provided by NopCommerce for an example and adapt accordingly.

#How to Customize
1. In the class CustomAuthenticationDefaults in the Domain constant, replace <yourdomain> with your Auth0 domain.
2. In the IsEmployeeSpecification class, replace <yourdomain> with your own email address domain, or add any other criteria to define your employees per your custom business.
3. In the CustomAuthenticationRegistrar class, edit the logoutUri variable and replace <yourdomain> with the domain of your Auth0 implementation. You may also need to replace the ResponseType and Scope to fit your own Auth0 implementation.
4. The default login is via /adminlogin - if you want a different endpoint, configure it using the RouteProvider class.
5. In the CustomAuthenticationController, the LoginCallback is called when Auth0 authenticates the user and replies back to the main application. This is when we then take the OAuth token to authenticate the user into NopCommerce. For this we need the email address of the Auth0 user. So you'll need to customize the GetEmailClaim for your use case. The default location is in the Principal object of the authenticateResult. However, depending on your configuration, it may be in other claim locations. Customize to fit your needs. If you're not sure, put a breakpoint here, inspect the AuthenticateResult object and determine where things are at.