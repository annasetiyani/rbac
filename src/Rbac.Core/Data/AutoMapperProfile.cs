using AutoMapper;
using Rbac.Core.Data.Entities;
using Rbac.Core.Data.Models;

namespace Rbac.Core.Data;

public class AutoMapperProfile:Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserItem>().ReverseMap()
            .ForMember(x => x.UserRoles, opt => opt.Ignore());
        CreateMap<Role, RoleItem>().ReverseMap()
            .ForMember(x => x.RolePermissions, opt => opt.Ignore()); ;
        CreateMap<Permission, PermissionItem>().ReverseMap()
            .ForMember(x=>x.RolePermissions, opt=>opt.Ignore());
    }

}
