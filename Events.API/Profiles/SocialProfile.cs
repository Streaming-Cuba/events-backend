using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class EventProfile : Profile 
    {
        public EventProfile() 
        {
            CreateMap<SocialCreateDTO, Social>();
            CreateMap<GroupItemTypeCreateDTO, GroupItemType>();
            CreateMap<NEventStatusCreateDTO, NEventStatus>();
            CreateMap<InteractionCreateDTO, Interaction>();
        }
    }
}