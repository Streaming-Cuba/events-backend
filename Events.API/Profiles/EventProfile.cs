using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class EventProfile : Profile 
    {
        public EventProfile() 
        {
            CreateMap<GroupCreateDTO, Group>().ReverseMap();
            CreateMap<SocialCreateDTO, Social>().ReverseMap();
            CreateMap<NTagCreateDTO, NTag>().ReverseMap();
            CreateMap<GroupItemTypeCreateDTO, GroupItemType>().ReverseMap();
            CreateMap<NEventStatusCreateDTO, NEventStatus>().ReverseMap();
            CreateMap<InteractionCreateDTO, Interaction>().ReverseMap();
            CreateMap<NCategoryCreateDTO, NCategory>().ReverseMap();
            CreateMap<SocialPlatformTypeCreateDTO, SocialPlatformType>().ReverseMap();

            CreateMap<EventCreateDTO, Event>();
            CreateMap<Event, EventCreateDTO>().ForMember(d => d.TagsId, cfg => cfg.MapFrom(s => s.Tags));

            CreateMap<EventTag, int>().ConstructUsing(x => x.TagId);
            CreateMap<EventSocial, int>().ConstructUsing(x => x.SocialId);
        }
    }
}