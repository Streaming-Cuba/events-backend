using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class AccountProfile : Profile 
    {
        public AccountProfile() 
        {
            CreateMap<AccountCreateDTO, Account>();
            CreateMap<Account, AccountCreateDTO>().ForMember(d => d.RolesId,
                                                           cfg => cfg.MapFrom(m => m.Roles));

            // flattening without context roles
            CreateMap<Account, AccountReadDTO>().ForMember(d => d.Name,
                                                           cfg => cfg.MapFrom(m => m.Roles));
            CreateMap<AccountRole, string>().ConvertUsing(x => x.Role.Name);
            
            CreateMap<RoleCreateDTO, Role>();
        }
    }
}