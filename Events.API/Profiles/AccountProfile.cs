using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class AccountProfile : Profile 
    {
        public AccountProfile() 
        {
            CreateMap<AccountCreateDTO, Account>();
            CreateMap<Account, AccountReadDTO>();
        }
    }
}