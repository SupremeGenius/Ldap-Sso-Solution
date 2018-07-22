using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LdapSso.DirectoryAccess
{
    public class LdapSsoDal
    {
        public static object AutheticateUserLdap(string username, string password)
        {
            try
            {
                AppSettingsReader appSettingReader = new AppSettingsReader();
                
                string servername = Convert.ToString(appSettingReader.GetValue("ServerName", typeof(string)));
                string container = Convert.ToString(appSettingReader.GetValue("Container", typeof(string)));
                string userContainer = Convert.ToString(appSettingReader.GetValue("UserContainer", typeof(string)));
                username = "CN=" + username + "," + userContainer;
                
                using (var pc = new PrincipalContext(ContextType.ApplicationDirectory, servername, container, ContextOptions.SimpleBind, username, password))
                {
                    UserPrincipal up = UserPrincipal.FindByIdentity(pc, IdentityType.DistinguishedName, username);

                    if (!string.IsNullOrEmpty(up.DistinguishedName))
                        return up.Guid;

                    return up.Guid;
                }
            }
            catch
            {
                return null;
            }
                        
        }
    }
}
