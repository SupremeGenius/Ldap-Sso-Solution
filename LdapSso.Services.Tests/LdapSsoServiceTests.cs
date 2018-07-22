using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Microsoft.QualityTools.Testing.Fakes;
using System.Collections.Generic;
using LdapSso.Services.Dtos;

namespace LdapSso.Services.Tests
{
    [TestClass]
    public class LdapSsoServiceTests
    {
        private const string TEST_USERNAME = "LdapUser";
        private const string TEST_PASSWORD = "LdapPassword";

        [TestMethod]
        public void AuthenticateUser_LdapSsoDalAutheticateUser_Success_ReturnsSuccess()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.AutheticateUserStringString = (a, b) => { return AuthenticationResult.SUCCESS; };
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.RenewLeaseGuidStringString = (a, b, c) => { return true; };
                LdapSsoService ldapSsoService = new LdapSsoService();

                // Act
                AuthenticationResult actualResult = ldapSsoService.AuthenticateUser(TEST_USERNAME, TEST_PASSWORD);

                // Assert
                actualResult.ShouldBe(AuthenticationResult.SUCCESS); 
            }
        }

        [TestMethod]
        public void AuthenticateUser_LdapSsoDalAutheticateUser_UserDoesNotExistsLdapValidGuid_ReturnsSuccess()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.AutheticateUserStringString = (a, b) => { return AuthenticationResult.USER_DOES_NOT_EXISTS; };
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.RenewLeaseGuidStringString = (a, b, c) => { return true; };
                LdapSso.DirectoryAccess.Fakes.ShimLdapSsoDal.AutheticateUserLdapStringString = (a, b) => { return Guid.NewGuid(); };

                LdapSsoService ldapSsoService = new LdapSsoService();

                // Act
                AuthenticationResult actualResult = ldapSsoService.AuthenticateUser(TEST_USERNAME, TEST_PASSWORD);

                // Assert
                actualResult.ShouldBe(AuthenticationResult.SUCCESS);
            }
        }

        [TestMethod]
        public void AuthenticateUser_LdapSsoDalAutheticateUser_UserDoesNotExistsLdapGuidNull_InvalidCredentails()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.AutheticateUserStringString = (a, b) => { return AuthenticationResult.USER_DOES_NOT_EXISTS; };
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.RenewLeaseGuidStringString = (a, b, c) => { return true; };
                LdapSso.DirectoryAccess.Fakes.ShimLdapSsoDal.AutheticateUserLdapStringString = (a, b) => { return null; };

                LdapSsoService ldapSsoService = new LdapSsoService();

                // Act
                AuthenticationResult actualResult = ldapSsoService.AuthenticateUser(TEST_USERNAME, TEST_PASSWORD);

                // Assert
                actualResult.ShouldBe(AuthenticationResult.INVALID_CREDENTIALS);
            }
        }

        [TestMethod]
        public void AuthenticateUser_LdapSsoDalAutheticateUser_Success_ReturnsSucess()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.AutheticateUserStringString = (a, b) => { return AuthenticationResult.SUCCESS; };
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.RenewLeaseGuidStringString = (a, b, c) => { return true; };
                LdapSso.DirectoryAccess.Fakes.ShimLdapSsoDal.AutheticateUserLdapStringString = (a, b) => { return null; };

                LdapSsoService ldapSsoService = new LdapSsoService();

                // Act
                AuthenticationResult actualResult = ldapSsoService.AuthenticateUser(TEST_USERNAME, TEST_PASSWORD);

                // Assert
                actualResult.ShouldBe(AuthenticationResult.SUCCESS);
            }
        }

        [TestMethod]
        public void GetUserGroups_UserName_ReturnsGroupNames()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.GetUserGroupsString = (a) => 
                {
                    List<DataAccess.UserGroup> userGroups = new List<DataAccess.UserGroup>();

                    userGroups.Add(new DataAccess.UserGroup()
                    {
                        Id = 1,
                        UserId = 1,
                        GroupId = 1,
                        Group = new DataAccess.Group()
                        {
                            GroupName = "administrator",
                            Id = 1
                        }
                    });

                    return userGroups;
                };

                LdapSso.DataAccess.Fakes.ShimLdapSsoDal.RenewLeaseGuidStringString = (a, b, c) => { return true; };
                LdapSso.DirectoryAccess.Fakes.ShimLdapSsoDal.AutheticateUserLdapStringString = (a, b) => { return null; };

                LdapSsoService ldapSsoService = new LdapSsoService();

                // Act
                UserGroupDto actualResult = ldapSsoService.GetUserGroups(TEST_USERNAME);

                // Assert
                actualResult.ShouldNotBeNull();
                actualResult.GroupNames.Length.ShouldBe(1);
            }
        }
    }
}
