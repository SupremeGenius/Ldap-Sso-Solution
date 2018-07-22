using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.DirectoryServices.AccountManagement.Fakes;
using LdapSso.DirectoryAccess;
using Shouldly;
using Microsoft.QualityTools.Testing.Fakes;

namespace LdapSso.DirectoryAccess.Tests
{
    [TestClass]
    public class LdapSsoDalTests
    {
        [TestMethod]
        public void AutheticateUserLdap_ReturnsValidGuid()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                ShimPrincipalContext.ConstructorContextTypeStringStringContextOptionsStringString = (a, b, c, d, e, f, g) => { new ShimPrincipalContext(); };
                ShimUserPrincipal.FindByIdentityPrincipalContextIdentityTypeString = (a, b, c) =>
                {
                    return new ShimUserPrincipal();
                };
                ShimPrincipal.AllInstances.DistinguishedNameGet = (a) => { return "test"; };
                ShimPrincipal.AllInstances.GuidGet = (a) => { return Guid.NewGuid(); };

                // Act
                var actualReturn = LdapSsoDal.AutheticateUserLdap(string.Empty, string.Empty);

                // Assert
                actualReturn.ShouldNotBeNull();
                actualReturn.ShouldBeOfType(typeof(Guid));
            }

        }

        [TestMethod]
        public void AutheticateUserLdap_FindByIdentityThrowsException_ReturnsNull()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                ShimPrincipalContext.ConstructorContextTypeStringStringContextOptionsStringString = (a, b, c, d, e, f, g) => { new ShimPrincipalContext(); };
                ShimUserPrincipal.FindByIdentityPrincipalContextIdentityTypeString = (a, b, c) =>
                {
                    throw new ApplicationException("This is test exception");
                };
                ShimPrincipal.AllInstances.DistinguishedNameGet = (a) => { return "test"; };
                ShimPrincipal.AllInstances.GuidGet = (a) => { return Guid.NewGuid(); };

                // Act
                var actualReturn = LdapSsoDal.AutheticateUserLdap(string.Empty, string.Empty);

                // Assert
                actualReturn.ShouldBeNull();                
            }

        }
    }
}
