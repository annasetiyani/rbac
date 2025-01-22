using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rbac.Core.Data;
using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;
using Rbac.Core.Services.Interfaces;

namespace Rbac.Core.Services.Providers;
public class UserProvider(AppDbContext dbContext, IMapper mapper, IRoleProvider roleProvider) 
    : IUserProvider
{
    private readonly AppDbContext dbContext = dbContext;
    private readonly IMapper mapper = mapper;
    private readonly IRoleProvider roleProvider = roleProvider;

    public async Task<Guid> CreateUserAsync(UserCreateRequest request, string hashedPassword)
    {
        var user = new User
        {
            Username = request.Username,
            PasswordHash = hashedPassword,
            Id = Guid.NewGuid(),
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await dbContext.Users.AddAsync(user);

        //add UserRole
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = request.RoleId,
        };
        await dbContext.UserRoles.AddAsync(userRole);

        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<UserItem> GetUserByIdAsync(Guid id)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x=>x.Id == id);

        return user != null ? await ToUserItemAsync(user): null!;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<IList<UserItem>> GetUsersAsync()
    {
        var users  = await dbContext.Users
            .Include(nameof(User.UserRoles))
            .AsNoTracking()
            .ToListAsync();
        if(users == null || !users.Any())
            return null!;

        var roleIds = users.SelectMany(x => x.UserRoles.Select(x => x.RoleId)).Distinct().ToList();
        var rolePermissions = await roleProvider.GetRolePermissionItemsAsync(roleIds);
        if (rolePermissions == null || !rolePermissions.Any())
            return users.Select(x => mapper.Map<UserItem>(x)).ToList();

        var results = new List<UserItem>();
        foreach (var user in users)
        {
            var userItem = mapper.Map<UserItem>(user);
            if(user.UserRoles == null || !user.UserRoles.Any())
            {
                results.Add(userItem);
                continue;
            }

            var userRoleIds = user.UserRoles.Select(x => x.RoleId).Distinct().ToList();

            userItem.RolePermissions = rolePermissions
                .Where(x => userRoleIds.Contains(x.Id)).ToList();

            results.Add(userItem);
        }
        return results?.OrderBy(x => x.Username).ToList() ?? null!;
    }

    private async Task<UserItem> ToUserItemAsync(User user)
    {
        var result = mapper.Map<UserItem>(user);
        var userRoles = await dbContext.UserRoles
            .AsNoTracking()
            .Include(nameof(UserRole.Role))
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        if (userRoles == null || !userRoles.Any())
            return result;

        result.RolePermissions = await roleProvider.GetRolePermissionItemsAsync(userRoles.Select(x => x.RoleId));

        return result;
    }

}
