using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LdapSso.DataAccess.Fakes;
using System.Data.Entity;
using System.Data.Entity.Fakes;
using Shouldly;
using Microsoft.QualityTools.Testing.Fakes;
using System.Collections.Generic;
using System.Linq;
using Moq;

namespace LdapSso.DataAccess.Tests
{
    [TestClass]
    public class LdapSsoDalTests
    {
        private const string TEST_USERNAME = "pratik";
        private const string TEST_PASSWORD = "cGFzc3dvcmQ=";

        [TestMethod]
        public void AutheticateUser_CorrectUserRenewLease_ReturnsSuccess()
        {
            using (ShimsContext.Create())
            {
                // Act
                AuthenticationResult actualResult = LdapSsoDal.AutheticateUser(TEST_USERNAME, string.Empty);

                // Assert
                actualResult.ShouldBe(AuthenticationResult.SUCCESS);
            }
        }

        [TestMethod]
        public void AutheticateUser_CorrectUserIncorrectPassword_ReturnsSuccess()
        {
            using (ShimsContext.Create())
            {
                // Act
                AuthenticationResult actualResult = LdapSsoDal.AutheticateUser(TEST_USERNAME, TEST_PASSWORD);

                // Assert
                actualResult.ShouldBe(AuthenticationResult.SUCCESS);
            }
        }

        [TestMethod]
        public void GetUserGroups_ExecutesFine()
        {
            using (ShimsContext.Create())
            {
                // Act
                var actualResult = LdapSsoDal.GetUserGroups(TEST_USERNAME);

                // Assert
                actualResult.ShouldNotBeNull();
                actualResult.Count.ShouldBe(1);
            }
        }

        [TestMethod]
        public void RenewLease_UserExistsLeaseExists_ExecutesFine()
        {
            using (ShimsContext.Create())
            {
                // Act
                var actualResult = LdapSsoDal.RenewLease(Guid.NewGuid(), TEST_USERNAME, TEST_PASSWORD);

                // Assert
                actualResult.ShouldNotBeNull();
            }
        }

        [TestMethod]
        public void RenewLease_UserDoesNotExists_ExecutesFine()
        {
            using (ShimsContext.Create())
            {
                // Act
                var actualResult = LdapSsoDal.RenewLease(Guid.NewGuid(), "PP", TEST_PASSWORD);

                // Assert
                actualResult.ShouldNotBeNull();
            }
        }
    }
}
