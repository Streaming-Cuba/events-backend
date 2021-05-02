using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class SubscriberProfile : Profile 
    {
        public SubscriberProfile() 
        {
            CreateMap<SubscriberCreateDTO, Subscriber>();      
        }
    }
}