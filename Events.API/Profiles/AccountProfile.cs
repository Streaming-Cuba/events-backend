using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class PermissionProfile : Profile 
    {
        public PermissionProfile() 
        {
            CreateMap<AccountCreateDTO, Account>();
            CreateMap<Account, AccountReadDTO>();
        }
    }
}