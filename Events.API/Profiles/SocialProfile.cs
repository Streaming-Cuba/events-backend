using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class EventProfile : Profile 
    {
        public EventProfile() 
        {
            CreateMap<SocialCreateDTO, Social>();
            CreateMap<NTagCreateDTO, NTag>();
            CreateMap<GroupItemTypeCreateDTO, GroupItemType>();
            CreateMap<NEventStatusCreateDTO, NEventStatus>();
            CreateMap<InteractionCreateDTO, Interaction>();
            CreateMap<NCategoryCreateDTO, NCategory>();
            CreateMap<SocialPlatformTypeCreateDTO, SocialPlatformType>();
            CreateMap<EventCreateDTO, Event>();
        }
    }
}