using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LdapSso.TestApplication2
{
    public partial class MainForm : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Response.StatusCode == 200)
            {
                Response.Write("Authentication Succes!");

                var loggedInUser = Convert.ToString(HttpContext.Current.Items["LoggedInUser"]);
                var userGroups = (string[])HttpContext.Current.Items["UserGroups"];

                Response.Write("The current Logged in user is : " + loggedInUser);

                Response.Write("<br />");

                Response.Write("The user is the part of the following groups : ");

                foreach (var userGroup in userGroups)
                {
                    Response.Write(userGroup);
                    Response.Write("<br />");
                }
            }
        }
    }
}