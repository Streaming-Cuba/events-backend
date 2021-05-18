using AutoMapper;
using Events.API.DTO;
using Events.API.Models;

namespace Events.API.Profiles {
    public class EventProfile : Profile 
    {
        public EventProfile() 
        {
            CreateMap<GroupCreateDTO, Group>();
            CreateMap<SocialCreateDTO, Social>();
            CreateMap<NTagCreateDTO, NTag>();
            CreateMap<GroupItemTypeCreateDTO, GroupItemType>();
            CreateMap<NEventStatusCreateDTO, NEventStatus>();
            CreateMap<InteractionCreateDTO, Interaction>();
            CreateMap<NCategoryCreateDTO, NCategory>();
            CreateMap<SocialPlatformTypeCreateDTO, SocialPlatformType>();
            CreateMap<EventCreateDTO, Event>();

            // reverse
            CreateMap<Group, GroupCreateDTO>();
            CreateMap<Social, SocialCreateDTO>();
            CreateMap<NTag, NTagCreateDTO>();
            CreateMap<GroupItemType, GroupItemTypeCreateDTO>();
            CreateMap<NEventStatus, NEventStatusCreateDTO>();
            CreateMap<Interaction, InteractionCreateDTO>();
            CreateMap<NCategory, NCategoryCreateDTO>();
            CreateMap<SocialPlatformType, SocialPlatformTypeCreateDTO>();

            CreateMap<Event, EventCreateDTO>().ForMember(d => d.TagsId, cfg => cfg.MapFrom(s => s.Tags))
                                              .ForMember(d => d.CategoryId, cfg => cfg.MapFrom(s => s.Category.Id))
                                              .ForMember(d => d.StatusId, cfg => cfg.MapFrom(s => s.Status.Id));


            CreateMap<EventTag, int>().ConstructUsing(x => x.TagId);
            CreateMap<EventSocial, int>().ConstructUsing(x => x.SocialId);
        }
    }
}