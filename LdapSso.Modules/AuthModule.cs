using LdapSso.Modules.LdapSsoClient;
using System;
using System.Security.Principal;
using System.Text;
using System.Web;
public class AuthModule : IHttpModule
{
    public AuthModule()
    {
    }

    public String ModuleName
    {
        get { return "AuthModule"; }
    }

    public void Init(HttpApplication application)
    {
        application.BeginRequest +=
            (new EventHandler(this.Application_BeginRequest));
    }

    private void Application_BeginRequest(Object source,
         EventArgs e)
    {
        try
        {
            Run(source, e);
        }
        catch
        {
            throw;
        }
    }

    public void Run(Object source, EventArgs e)
    {
        // Get the reference of the application and context server classes
        HttpApplication application = (HttpApplication)source;
        HttpContext context = application.Context;

        // Get the current identity of the windows users
        var identity = WindowsIdentity.GetCurrent();
        var user = identity.Name.Split("\\".ToCharArray());

        LdapSsoServiceClient ldapSsoServiceClient = new LdapSsoServiceClient();

        // Execute the service call to authenticate user
        AuthenticationResult result = ldapSsoServiceClient.AuthenticateUser(user[1], string.Empty);

        // Return the appropriate staus code - 200 - Success,  403 - Forbidden
        if (result != AuthenticationResult.SUCCESS)
        {
            HttpContext.Current.Response.Redirect("http://localhost:6011/Account/Index");
            HttpContext.Current.Response.StatusCode = 403;
        }
        else
        {
            // Execute the service call and get the user groups
            UserGroupDto userGroups = ldapSsoServiceClient.GetUserGroups(user[1]);

            // Persists the information in the session variables
            context.Items.Add("LoggedInUser", identity.Name);
            context.Items.Add("UserGroups", userGroups.GroupNames);

            HttpContext.Current.Response.StatusCode = 200;
        }
    }

    public void Dispose() { }
}