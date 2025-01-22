
using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;

namespace Rbac.Core.Services.Interfaces;

public interface IPermissionProvider
{
    Task<IList<PermissionItem>?> GetPermissionItemsAsync();
    Task<IList<PermissionItem>?> GetPermissionsAsync(IEnumerable<Guid> ids);
    Task<PermissionItem> GetPermissionItemAsync(Guid id);
}
