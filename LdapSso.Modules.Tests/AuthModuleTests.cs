using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.Web.Fakes;
using System.Collections.Generic;
using Microsoft.QualityTools.Testing.Fakes;
using Shouldly;

namespace LdapSso.Modules.Tests
{
    [TestClass]
    public class AuthModuleTests
    {
        [TestMethod]
        public void AuthModule_Run_ExecutesFine()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                AuthModule authModule = new AuthModule();
                ShimHttpApplication.AllInstances.ContextGet = (a) =>
                {
                    return new ShimHttpContext()
                    {
                        ItemsGet = () =>
                        {
                            return new Dictionary<object, object>();
                        }
                    };   
                };
                ShimHttpContext.CurrentGet = () =>
                {
                    return new ShimHttpContext();
                };
                ShimHttpContext.AllInstances.ResponseGet = (a) =>
                {
                    return new ShimHttpResponse()
                    {
                        StatusCodeGet = () => { return 200; }
                    };
                };                    

                // Act
                authModule.Run(new HttpApplication(), new EventArgs());

                // Assert 
                HttpContext.Current.Response.StatusCode.ShouldBe(200);
            }
        }
    }
}
