using DotnetBase.Domain.Entities;
using System.Collections.Generic;

namespace DotnetBase.Infrastructure.Common.Constants
{
    public static class RoleConstants
    {
        public const string ADMIN = "Admin";
        public const string USER = "User";

        public static List<ApplicationRole> GetListRoles()
        {
            return new List<ApplicationRole>() {
                new ApplicationRole{
                    Name = "Admin",
                    NormalizedName = "Admin"
                },
                new ApplicationRole {
                    Name = "User",
                    NormalizedName = "User"
                }
            };
        }
    }
}
