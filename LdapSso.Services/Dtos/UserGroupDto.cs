using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LdapSso.Services.Dtos
{
    public class UserGroupDto
    {
        public int UserId { get; set; }

        public string[] GroupNames { get; set; }
    }
}