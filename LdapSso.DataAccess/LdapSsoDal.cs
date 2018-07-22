using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LdapSso.DataAccess
{
    public class LdapSsoDal
    {
        public static AuthenticationResult AutheticateUser(string username, string password)
        {
            try
            {
                using (LdapSsoDBEntities ldapSsoDB = new LdapSsoDBEntities())
                {
                    // Query if the user exists. 
                    // This is internal to the system.  The external user is not aware about the exact error, for security reasons
                    var user = ldapSsoDB.Users.FirstOrDefault(a => a.UserName == username);

                    if (user == null)
                        return AuthenticationResult.USER_DOES_NOT_EXISTS;

                    // Query if the user exists and password is blank - which means the call is executed from HttpModule
                    user = ldapSsoDB.Users.FirstOrDefault(a => a.UserName == username && password == string.Empty);

                    if (user != null)
                    {
                        var userLease = ldapSsoDB.UserLeases.FirstOrDefault(a => a.ExpiresOn <= DateTime.Now && a.User.UserName == username);

                        if (userLease == null)
                            return AuthenticationResult.SUCCESS;

                        return AuthenticationResult.INVALID_CREDENTIALS;
                    }

                    // Query if the user and password matches. 
                    // This is internal to the system.  The external user is not aware about the exact error, for security reasons
                    user = ldapSsoDB.Users.FirstOrDefault(a => a.UserName == username && a.Password == password);

                    if (user == null)
                        return AuthenticationResult.INVALID_CREDENTIALS;

                    return AuthenticationResult.SUCCESS;
                }
            }
            catch
            {
                throw;
            }           
        }

        public static List<UserGroup> GetUserGroups(string username)
        {
            try
            {
                // Get the groups associated to the user
                using (LdapSsoDBEntities ldapSsoDB = new LdapSsoDBEntities())
                {
                    return ldapSsoDB.UserGroups.Include("Group").Where(a => a.User.UserName == username).ToList();
                }
            }
            catch
            {
                throw;
            }
        }


        public static bool RenewLease(Guid objectGuid, string username, string password)
        {
            try
            {
                using (LdapSsoDBEntities ldapSsoDB = new LdapSsoDBEntities())
                {
                    // If the user is new so create the record in User table.  This workflow executes for the first time AD auth is executed
                    var user = ldapSsoDB.Users.FirstOrDefault(a => a.UserName == username);

                    var encryptedPassword = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

                    if (user == null)
                    {
                        user = new User()
                        {
                            ObjectGuid = objectGuid.ToString(),
                            UserName = username,
                            Password = encryptedPassword
                        };

                        ldapSsoDB.Users.Add(user);

                        var userGroups = new UserGroup()
                        {
                            UserId = user.Id,
                            GroupId = 1
                        };

                        ldapSsoDB.UserGroups.Add(userGroups);

                        ldapSsoDB.SaveChanges();
                    }

                    var userLease = ldapSsoDB.UserLeases.FirstOrDefault(a => a.UserId == user.Id);

                    if (userLease == null)
                    {
                        // If the user does not have lease associated with it then create an entry in the UserLease table

                        userLease = new UserLease()
                        {
                            UserId = user.Id,
                            ExpiresOn = DateTime.Now.AddDays(7)
                        };

                        ldapSsoDB.UserLeases.Add(userLease);
                    }
                    else
                    {
                        // Update the lease to 7 more days

                        userLease.ExpiresOn = DateTime.Now.AddDays(7);
                    }

                    ldapSsoDB.SaveChanges();

                    return true;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
