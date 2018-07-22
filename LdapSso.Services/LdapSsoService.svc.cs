using LdapSso.DataAccess;
using LdapSso.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace LdapSso.Services
{
    public class LdapSsoService : ILdapSsoService
    {
        /// <summary>
        /// This function authenticates the user against local database and AD LDS instance
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public AuthenticationResult AuthenticateUser(string username, string password)
        {
            try
            {
                // Step 1: Check the user in the local database
                AuthenticationResult ret = LdapSso.DataAccess.LdapSsoDal.AutheticateUser(username, password);

                if (ret == AuthenticationResult.USER_DOES_NOT_EXISTS)
                {
                    // Step 2:  If the user does not exists in the local database then get from the AD LDS (ADAM) instance
                    object objectGuid = LdapSso.DirectoryAccess.LdapSsoDal.AutheticateUserLdap(username, password);

                    // Check if the AD LDS (ADAM) user is valid. If not then return invalid credentails
                    if (objectGuid == null)
                        return AuthenticationResult.INVALID_CREDENTIALS;

                    // Step 3:  Renew the lease period of the user.  AFter the lease expires the user data needs to be fetched from AD LDS again
                    LdapSso.DataAccess.LdapSsoDal.RenewLease(new Guid(Convert.ToString(objectGuid)), username, password);

                    return AuthenticationResult.SUCCESS;
                }
                else if (ret == AuthenticationResult.SUCCESS)
                {
                    // If the user authentication is successful then renew the lease of the user
                    LdapSso.DataAccess.LdapSsoDal.RenewLease(Guid.NewGuid(), username, password);
                }

                return ret;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// This function gets the user groups
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserGroupDto GetUserGroups(string username)
        {
            try
            {
                List<UserGroup> userGroups = LdapSso.DataAccess.LdapSsoDal.GetUserGroups(username);
                UserGroupDto userGroupDto = new UserGroupDto();

                userGroupDto.GroupNames = userGroups.Select(a => a.Group.GroupName).ToArray();

                return userGroupDto;
            }
            catch
            {
                throw;
            }
        }      

    }
}
