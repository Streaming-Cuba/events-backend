using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventContext _context;
        private readonly IMapper _mapper;

        public EventController(EventContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region Create models and push to database
        [HttpPost]
        [Route("/api/v1/[controller]/social")]
        public async Task<ActionResult<Event>> CreateSocial([FromBody] Social social)
        {
            await _context.Socials.AddAsync(social);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), social);
        }

        [HttpPost]
        [Route("/api/v1/[controller]/socials")]
        public async Task<ActionResult<SocialPlatformType>> CreateSocialPlatformType([FromBody] SocialPlatformType socialPlatformType)
        {
            await _context.SocialPlatformTypes.AddAsync(socialPlatformType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocialPlatformType), socialPlatformType);
        }

        [HttpPost]
        [Route("/api/v1/[controller]/category")]
        public async Task<ActionResult<NCategory>> CreateCategory([FromBody] NCategory category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), category);
        }

        [HttpPost]
        [Route("/api/v1/[controller]/interation")]
        public async Task<ActionResult<Interation>> CreateInteration([FromBody] Interation interation)
        {
            await _context.Interations.AddAsync(interation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateInteration), interation);
        }

        [HttpPost]
        [Route("/api/v1/[controller]/eventstatus")]
        public async Task<ActionResult<NEventStatus>> CreateEventStatus([FromBody] NEventStatus eventStatus)
        {
            await _context.EventStatuses.AddAsync(eventStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEventStatus), eventStatus);
        }

        [HttpPost]
        [Route("/api/v1/[controller]/group/itemtype")]
        public async Task<ActionResult<GroupItemType>> CreateGroupItemType([FromBody] GroupItemType groupItemType)
        {
            await _context.GroupItemTypes.AddAsync(groupItemType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGroupItemType), groupItemType);
        }
        #endregion
    }
}
