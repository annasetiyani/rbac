﻿
using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;

namespace Rbac.Core.Services.Interfaces;

public interface IUserProvider
{
    Task<bool> UpdateActiveAsync(Guid id, bool isActive);
    Task<Guid> CreateUserAsync(UserCreateRequest request, string hashedPassword);
    Task<bool> DeleteUserAsync(Guid id);
    Task<UserItem> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<IList<UserItem>> GetUsersAsync();
    Task<bool> UpdateUserAsync(UserUpdateRequest request);
    Task<bool> UpdatePasswordAsync(Guid id, string hashedPassword);
}
