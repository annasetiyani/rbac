using Rbac.Core.Data.Entities;

namespace Rbac.Core.Data;

public class SeederData(AppDbContext context)
{
    private readonly AppDbContext dbContext = context;

    public static async Task SeedDataAsync(AppDbContext dbContext)
    {
        dbContext.Database.EnsureCreated();
        var seeder = new SeederData(dbContext);
        await seeder.SeedPermissionsAsync();
        await seeder.SeedRolesAsync();
        await seeder.SeedRolePermissionsAsync();
    }
    private async Task SeedRolesAsync()
    {
        if (!dbContext.Roles.Any())
        {
            var roles = new List<Role>
            {
                new() { Id = Guid.NewGuid(), RoleName = "Admin" },
                new() { Id = Guid.NewGuid(), RoleName = "Manager" },
                new(){ Id = Guid.NewGuid(), RoleName = "User" }
            };
            await dbContext.Roles.AddRangeAsync(roles);
            dbContext.SaveChanges();
        }
    }
    private async Task SeedPermissionsAsync()
    {
        if (!dbContext.Permissions.Any())
        {
            var permissions = new List<Permission>
            {
                new() { Id = Guid.NewGuid(), PermissionName = "Create" },
                new() { Id = Guid.NewGuid(), PermissionName = "Read" },
                new() { Id = Guid.NewGuid(), PermissionName = "Update" },
                new() { Id = Guid.NewGuid(), PermissionName = "Delete" }
            };
            await dbContext.Permissions.AddRangeAsync(permissions);
            dbContext.SaveChanges();
        }
    }

    private async Task SeedRolePermissionsAsync()
    {
        if (!dbContext.RolePermissions.Any())
        {
            var adminRole = dbContext.Roles.FirstOrDefault(r => r.RoleName == "Admin");
            var managerRole = dbContext.Roles.FirstOrDefault(r => r.RoleName == "Manager");
            var userRole = dbContext.Roles.FirstOrDefault(r => r.RoleName == "User");

            var createPermission = dbContext.Permissions.FirstOrDefault(p => p.PermissionName == "Create");
            var readPermission = dbContext.Permissions.FirstOrDefault(p => p.PermissionName == "Read");
            var updatePermission = dbContext.Permissions.FirstOrDefault(p => p.PermissionName == "Update");
            var deletePermission = dbContext.Permissions.FirstOrDefault(p => p.PermissionName == "Delete");

            if (adminRole != null && createPermission != null && readPermission != null &&
                updatePermission != null && deletePermission != null)
            {
                await dbContext.RolePermissions.AddRangeAsync(
                    new RolePermission { RoleId = adminRole.Id, PermissionId = createPermission.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = readPermission.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = updatePermission.Id },
                    new RolePermission { RoleId = adminRole.Id, PermissionId = deletePermission.Id }
                );
            }

            if (managerRole != null && readPermission != null && updatePermission != null)
            {
                await dbContext.RolePermissions.AddRangeAsync(
                    new RolePermission { RoleId = managerRole.Id, PermissionId = readPermission.Id },
                    new RolePermission { RoleId = managerRole.Id, PermissionId = updatePermission.Id }
                );
            }

            if (userRole != null && readPermission != null)
            {
                await dbContext.RolePermissions.AddRangeAsync(
                    new RolePermission { RoleId = userRole.Id, PermissionId = readPermission.Id }
                );
            }
            dbContext.SaveChanges();
        }
        
    }
    
}
