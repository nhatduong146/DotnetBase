using DotnetBase.Application.Services.DataInitialize;
using DotnetBase.Domain.Entities;
using DotnetBase.Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace DotnetBase.Infrastructure.Services.DataInitialize
{
    public class UserDataInitializeService(UserManager<ApplicationUser> userManager) : IDataInitializeService
    {
        public int Order
        {
            get => 2;
            set
            {

            }
        }

        public async Task RunAsync()
        {
            const string password = "123456";

            var listMemberAccount = new List<ApplicationUser>
            {
                new()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    PhoneNumber = "+84935198553",
                    Name = "Admin"
                },
                new()
                {
                    UserName = "user@gmail.com",
                    Email = "user@gmail.com",
                    PhoneNumber = "+84935198235",
                    Name = "User"
                }
            };

            foreach (var user in listMemberAccount)
            {
                if (userManager.FindByEmailAsync(user.Email).Result != null) continue;

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, user.Name);
                }
            }
        }
    }
}
