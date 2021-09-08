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

            CreateMap<Event, EventReadDTO>().ForMember(d => d.Category, cfg => cfg.MapFrom(s => s.Category))
                                            .ForMember(d => d.Status, cfg => cfg.MapFrom(s => s.Status))
                                            .ForMember(d => d.Socials, cfg => cfg.MapFrom(s => s.Socials))
                                            .ForMember(d => d.Tags, cfg => cfg.MapFrom(s => s.Tags));
            CreateMap<NCategory, string>().ConvertUsing(x => x.Name);
            CreateMap<NEventStatus, string>().ConvertUsing(x => x.Name);
            CreateMap<EventSocial, string>().ConvertUsing(x => x.Social.Url);
            CreateMap<EventTag, int>().ConvertUsing(x => x.TagId);
        }
    }
}