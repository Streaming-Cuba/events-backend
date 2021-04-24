using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class SocialProfile : Profile 
    {
        public SocialProfile() 
        {
            CreateMap<SocialCreateDTO, Social>();
        }
    }
}