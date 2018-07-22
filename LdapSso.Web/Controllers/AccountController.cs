using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LdapSso.Web.LdapSsoClient;
using LdapSso.Web.Models;
using System.Web.Security;
using System.Text;

namespace LdapSso.Web.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserLoginModel userLoginModel)
        {
            LdapSsoServiceClient ldapSsoClient = new LdapSsoServiceClient();

            AuthenticationResult ret = ldapSsoClient.AuthenticateUser(userLoginModel.UserName, userLoginModel.Password);

            return new ContentResult()
            {
                Content = "The user has been successfully authenticated!,  Please navigate to the website again."
            };
        }        
    }
}
