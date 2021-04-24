using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Events.API.Controllers
{
    [ApiController]
    [Route("/api/v1/event")]
    public class EventController : ControllerBase
    {
        private readonly EventContext _context;
        private readonly IMapper _mapper;

        public EventController(EventContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<Event> ListEvents()
        {
            var events = _context.Events.ToList();
            return CreatedAtAction(nameof(ListEvents), events);
        }

        #region Create models and push to database
        [HttpPost("social")]
        public async Task<ActionResult<Event>> CreateSocial([FromBody] Social social)
        {
            await _context.Socials.AddAsync(social);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), social);
        }

        [HttpPost("socials")]
        public async Task<ActionResult<SocialPlatformType>> CreateSocialPlatformType([FromBody] SocialPlatformType socialPlatformType)
        {
            await _context.SocialPlatformTypes.AddAsync(socialPlatformType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocialPlatformType), socialPlatformType);
        }

        [HttpPost("category")]
        public async Task<ActionResult<NCategory>> CreateCategory([FromBody] NCategory category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), category);
        }

        [HttpPost("interation")]
        public async Task<ActionResult<Interation>> CreateInteration([FromBody] Interation interation)
        {
            await _context.Interations.AddAsync(interation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateInteration), interation);
        }

        [HttpPost("event-status")]
        public async Task<ActionResult<NEventStatus>> CreateEventStatus([FromBody] NEventStatus eventStatus)
        {
            await _context.EventStatuses.AddAsync(eventStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEventStatus), eventStatus);
        }

        [HttpPost("group/item-type")]
        public async Task<ActionResult<GroupItemType>> CreateGroupItemType([FromBody] GroupItemType groupItemType)
        {
            await _context.GroupItemTypes.AddAsync(groupItemType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGroupItemType), groupItemType);
        }
        #endregion
    }
}
