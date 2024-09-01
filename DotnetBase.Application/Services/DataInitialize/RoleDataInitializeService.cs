using DotnetBase.Domain.Entities;
using DotnetBase.Infrastructure.Common.Constants;
using Microsoft.AspNetCore.Identity;

namespace DotnetBase.Application.Services.DataInitialize
{
    public class RoleDataInitializeService(RoleManager<ApplicationRole> roleManager) : IDataInitializeService
    {
        public int Order
        {
            get => 1;
            set
            {

            }
        }

        public async Task RunAsync()
        {
            var listSystemRoles = RoleConstants.GetListRoles();
            var listRolesInDb = roleManager.Roles.Select(r => r.Name).ToList();
            var listRolesNotUsed = listRolesInDb.Except(listSystemRoles.Select(r => r.Name).ToList()).ToList();

            foreach (var role in listSystemRoles)
            {
                if (await roleManager.FindByNameAsync(role.Name) == null)
                {
                    await roleManager.CreateAsync(new ApplicationRole(role.Name));
                }
            }

            if (listRolesNotUsed.Any())
            {
                foreach (var role in listRolesNotUsed)
                {
                    var rolePrepareToDelete = await roleManager.FindByNameAsync(role);
                    await roleManager.DeleteAsync(rolePrepareToDelete);
                }
            }
        }
    }
}
