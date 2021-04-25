using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Events.API.DTO;
using Microsoft.EntityFrameworkCore;

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

        #region Get models information
        [HttpGet]
        public ActionResult<Event> ListEvents()
        {
            var list = _context.Events.ToList();
            return Ok(list);
        }

        [HttpGet("{identifier}")]
        public async Task<ActionResult<Event>> EventByIdentifier(string identifier)
        {
            var item = await _context.Events
                    .Include(d => d.Groups)
                    .ThenInclude(p => p.ChildGroups)

                    .Include(d => d.Groups)
                    .ThenInclude(p => p.Items)
                    .ThenInclude(p => p.Type)

                    .Include(d => d.Groups)
                    .ThenInclude(p => p.Items)
                    .ThenInclude(p => p.Metadata)

                    // second level
                    .Include(d => d.Groups)
                    .ThenInclude(p => p.ChildGroups)
                    .ThenInclude(p => p.Items)
                    .ThenInclude(p => p.Metadata)
                    .SingleOrDefaultAsync(x => x.Identifier == identifier);

            if (item == null)
                return NotFound();
            return Ok(item);
        }
        #endregion

        #region Create models and push to database
        [HttpPost("tag")]
        public async Task<ActionResult<NTag>> CreateTag([FromBody] NTagCreateDTO tag)
        {
            await _context.Tags.AddAsync(_mapper.Map<NTag>(tag));
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateTag), tag);
        }

        [HttpPost("social")]
        public async Task<ActionResult<Event>> CreateSocial([FromBody] SocialCreateDTO social)
        {
            var type = await _context.SocialPlatformTypes.FindAsync(social.PlatformTypeId);
            if (type == null)
                return NotFound();

            var _social = _mapper.Map<Social>(social);
            _social.PlatformType = type;

            await _context.Socials.AddAsync(_social);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), social);
        }

        [HttpPost("social/platform-type")]
        public async Task<ActionResult<SocialPlatformType>> CreateSocialPlatformType([FromBody] SocialPlatformTypeCreateDTO socialPlatformType)
        {
            await _context.SocialPlatformTypes.AddAsync(_mapper.Map<SocialPlatformType>(socialPlatformType));
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

        [HttpPost("interaction")]
        public async Task<ActionResult<Interaction>> CreateInteraction([FromBody] InteractionCreateDTO interaction)
        {
            await _context.Interactions.AddAsync(_mapper.Map<Interaction>(interaction));
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateInteraction), interaction);
        }

        [HttpPost("event-status")]
        public async Task<ActionResult<NEventStatus>> CreateEventStatus([FromBody] NEventStatusCreateDTO eventStatus)
        {
            await _context.EventStatuses.AddAsync(_mapper.Map<NEventStatus>(eventStatus));
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEventStatus), eventStatus);
        }

        [HttpPost("group/item-type")]
        public async Task<ActionResult<GroupItemType>> CreateGroupItemType([FromBody] GroupItemTypeCreateDTO groupItemType)
        {
            await _context.GroupItemTypes.AddAsync(_mapper.Map<GroupItemType>(groupItemType));
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGroupItemType), groupItemType);
        }
        #endregion

        #region Modify models 
        [HttpPut("social/{id}")]
        public async Task<ActionResult> EditSocial([FromRoute] int id,
                                                          [FromBody] SocialCreateDTO social)
        {
            var _social = await _context.Socials.FindAsync(id);
            if (_social == null)
                return NotFound(id);

            if (social.PlatformTypeId.HasValue)
            {
                var type = await _context.SocialPlatformTypes.FindAsync(social.PlatformTypeId);
                if (type == null)
                    return BadRequest();
                _social.PlatformType = type;
            }

            _social = _mapper.Map<SocialCreateDTO, Social>(social, _social);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("group/item-type/{id}")]
        public async Task<ActionResult> EditGroupItemType([FromRoute] int id,
                                                                         [FromBody] GroupItemTypeCreateDTO groupItemType)
        {
            var _groupItemType = await _context.GroupItemTypes.FindAsync(id);
            if (_groupItemType == null)
                return NotFound(id);

            _groupItemType = _mapper.Map<GroupItemTypeCreateDTO, GroupItemType>(groupItemType, _groupItemType);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("event-status/{id}")]
        public async Task<ActionResult> EditEventStatus([FromRoute] int id,
                                                        [FromBody] NEventStatusCreateDTO eventStatus)
        {
            var _eventStatus = await _context.EventStatuses.FindAsync(id);
            if (_eventStatus == null)
                return NotFound(id);

            _eventStatus = _mapper.Map<NEventStatusCreateDTO, NEventStatus>(eventStatus, _eventStatus);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("interaction/{id}")]
        public async Task<ActionResult> EditInteraction([FromRoute] int id,
                                                        [FromBody] InteractionCreateDTO interaction)
        {
            var _interaction = await _context.Interactions.FindAsync(id);
            if (_interaction == null)
                return NotFound(id);

            _interaction = _mapper.Map<InteractionCreateDTO, Interaction>(interaction, _interaction);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("tag/{id}")]
        public async Task<ActionResult> EditTag([FromRoute] int id,
                                                [FromBody] NTagCreateDTO tag)
        {
            var _tag = await _context.Tags.FindAsync(id);
            if (_tag == null)
                return NotFound(id);

            _tag = _mapper.Map<NTagCreateDTO, NTag>(tag, _tag);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("category/{id}")]
        public async Task<ActionResult> EditCategory([FromRoute] int id,
                                                     [FromBody] NCategoryCreateDTO category)
        {
            var _category = await _context.Categories.FindAsync(id);
            if (_category == null)
                return NotFound(id);

            _category = _mapper.Map<NCategoryCreateDTO, NCategory>(category, _category);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("social/platform-type/{id}")]
        public async Task<ActionResult> EditSocialPlatformType([FromRoute] int id,
                                                               [FromBody] SocialPlatformTypeCreateDTO socialPlatformType)
        {
            var _socialPlatformType = await _context.SocialPlatformTypes.FindAsync(id);
            if (_socialPlatformType == null)
                return NotFound(id);

            _socialPlatformType = _mapper.Map<SocialPlatformTypeCreateDTO, SocialPlatformType>(
                socialPlatformType,
                _socialPlatformType);
            await _context.SaveChangesAsync();
            return Ok();
        }

        #endregion
    }
}
