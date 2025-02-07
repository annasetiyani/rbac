﻿using AutoMapper;
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
            IsActive = false, //default to false. User need to be activated. 
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await dbContext.Users.AddAsync(user);

        //add UserRoles
        foreach (var roleId in request.RoleIds)
        {
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }
        
        await dbContext.SaveChangesAsync();

        return user.Id;
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        var userRoles = await dbContext.UserRoles
            .Where(x => x.UserId == id)
            .ToListAsync();
        if (userRoles != null && userRoles.Count > 0)
        {
            dbContext.UserRoles.RemoveRange(userRoles);
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return false;
        dbContext.Users.Remove(user);

        await dbContext.SaveChangesAsync();
        return true;
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

    public async Task<bool> UpdateActiveAsync(Guid id, bool isActive)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return false;

        user.IsActive = isActive;
        user.UpdatedAt = DateTime.UtcNow;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(Guid id, string hashedPassword)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        if (user == null)
            return false;

        user.PasswordHash = hashedPassword;
        user.UpdatedAt = DateTime.UtcNow;
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateUserAsync(UserUpdateRequest request)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id);

        if (user == null)
            return false;
        
        user.Username = request.Username ?? user.Username;
        user.Email = request.Email ?? user.Email;
        user.UpdatedAt = DateTime.UtcNow;
        dbContext.Users.Update(user);

        var userRoles = await dbContext.UserRoles
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        if (userRoles != null && userRoles.Count != 0)
        {
            dbContext.UserRoles.RemoveRange(userRoles);
        }

        foreach (var roleId in request.RoleIds)
        {
            await dbContext.UserRoles.AddAsync(new UserRole
            {
                UserId = user.Id,
                RoleId = roleId
            });
        }

        await dbContext.SaveChangesAsync();
        return true;
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
