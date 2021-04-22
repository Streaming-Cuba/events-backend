using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class PermissionProfile : Profile 
    {
        public PermissionProfile() 
        {
            CreateMap<PermissionCreateDTO, Permission>();
            CreateMap<RoleCreateDTO, Role>();
            CreateMap<AccountCreateDTO, Account>();
            CreateMap<Account, AccountReadDTO>();
            CreateMap<Role, RoleReadDTO>();
        }
    }
}