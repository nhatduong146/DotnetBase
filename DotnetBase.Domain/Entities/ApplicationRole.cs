using Microsoft.AspNetCore.Identity;
using System;

namespace DotnetBase.Domain.Entities
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole() : base()
        {

        }

        public ApplicationRole(string rolename) : base(rolename)
        {

        }
    }
}
