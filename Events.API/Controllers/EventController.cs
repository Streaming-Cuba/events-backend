using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Events.API.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

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
        public async Task<ActionResult<Event>> ListEvents() => Ok(await _context.Events.ToListAsync());

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
        [HttpPost]
        public async Task<ActionResult> CreateEvent([FromBody] EventCreateDTO @event)
        {
            // validate data
            var _event = _mapper.Map<Event>(@event);

            _event.Category = await _context.Categories.FindAsync(@event.CategoryId);
            if (_event.Category == null)
                return BadRequest(new
                {
                    error = $"The category with id: {@event.CategoryId} don't exists"
                });

            _event.Status = await _context.EventStatuses.FindAsync(@event.StatusId);
            if (_event.Status == null)
                return BadRequest(new
                {
                    error = $"The status with id: {@event.StatusId} don't exists"
                });

            foreach (var tagId in @event.TagsId)
            {
                var tag = await _context.Tags.FindAsync(tagId);
                if (tag == null)
                    return BadRequest(new
                    {
                        error = $"The tag with id: {tagId} don't exists"
                    });
                _event.Tags.Add(new EventTag
                {
                    Tag = tag,
                    Event = _event
                });
            }

            _event.CreatedAt = DateTime.UtcNow;
            _event.ModifiedAt = DateTime.UtcNow;

            return CreatedAtAction(nameof(CreateEvent), new { id = _event.Id });
        }

        // [HttpPost("interaction/{eventId}")]
        // public async Task<ActionResult> CreateInteraction([FromBody] InteractionCreateDTO interaction) 
        // {

        // }

        [HttpPost("group/item/{groupId}")]
        public async Task<ActionResult> CreateGroupItem([FromRoute] int groupId,
                                                        [FromBody] GroupItemCreateDTO groupItem)
        {
            var _group = await _context.Groups.FindAsync(groupId);
            if (_group == null)
                return BadRequest(new
                {
                    error = $"The group with id: {groupId} don't exists"
                });


            var type = await _context.GroupItemTypes.FindAsync(groupItem.TypeId);
            if (type == null)
                return BadRequest(new
                {
                    error = $"The group item type with id: {groupItem.TypeId} don't exists"
                });

            var _groupItem = _mapper.Map<GroupItem>(groupItem);
            _groupItem.Type = type;
            _group.Items.Add(_groupItem);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("group/item/vote")]
        public async Task<ActionResult> CreateVote([FromQuery][Required] int groupItemId,
                                                   [FromQuery][Required] string typeName)
        {
            var vote = await _context.GroupItemVotes.FirstOrDefaultAsync(x => x.Type == typeName);

            if (vote == null)
            {
                vote = new GroupItemVote {
                    Count = 1,
                    Type = typeName
                };

                await _context.GroupItemVotes.AddAsync(vote);                
            } else {
                vote.Count++;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("group")]
        public async Task<ActionResult> CreateGroup([FromQuery] int? eventId,
                                                    [FromQuery] int? groupParentId,
                                                    [FromBody] GroupCreateDTO group)
        {
            // WARNING: Must set almost one id
            if (eventId == null && groupParentId == null)
                return BadRequest(new
                {
                    error = "Must specify event id or a group parent id to make the connection"
                });

            Group _group = _mapper.Map<Group>(group);
            if (eventId != null)
            {
                var _event = await _context.Events.FindAsync(eventId);
                if (_event != null)
                    return BadRequest(new
                    {
                        error = $"The event with id: {eventId} don't exists"
                    });
                _event.Groups.Add(_group);
            }
            if (groupParentId != null)
            {
                var _groupParent = await _context.Groups.FindAsync(groupParentId);
                if (_groupParent != null)
                    return BadRequest(new
                    {
                        error = $"The group with id: {groupParentId} don't exists"
                    });
                _groupParent.ChildGroups.Add(_group);
            }

            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGroup), new { id = _group.Id });
        }

        [HttpPost("tag")]
        public async Task<ActionResult> CreateTag([FromBody] NTagCreateDTO tag)
        {
            var _tag = _mapper.Map<NTag>(tag);
            await _context.Tags.AddAsync(_tag);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateTag), new { id = _tag.Id });
        }

        [HttpPost("social")]
        public async Task<ActionResult> CreateSocial([FromBody] SocialCreateDTO social)
        {
            var type = await _context.SocialPlatformTypes.FindAsync(social.PlatformTypeId);
            if (type == null)
                return BadRequest(new
                {
                    error = $"The platform type with id: {social.PlatformTypeId} don't exists"
                });

            var _social = _mapper.Map<Social>(social);
            _social.PlatformType = type;

            await _context.Socials.AddAsync(_social);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), new { id = _social.Id });
        }

        [HttpPost("social/platform-type")]
        public async Task<ActionResult> CreateSocialPlatformType([FromBody] SocialPlatformTypeCreateDTO socialPlatformType)
        {
            var _socialPlatformType = _mapper.Map<SocialPlatformType>(socialPlatformType);
            await _context.SocialPlatformTypes.AddAsync(_socialPlatformType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocialPlatformType), new { id = _socialPlatformType.Id });
        }

        [HttpPost("category")]
        public async Task<ActionResult> CreateCategory([FromBody] NCategoryCreateDTO category)
        {
            var _category = _mapper.Map<NCategory>(category);
            await _context.Categories.AddAsync(_category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), new { id = _category.Id });
        }

        [HttpPost("event-status")]
        public async Task<ActionResult> CreateEventStatus([FromBody] NEventStatusCreateDTO eventStatus)
        {
            var _eventStatus = _mapper.Map<NEventStatus>(eventStatus);
            await _context.EventStatuses.AddAsync(_eventStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEventStatus), new { id = _eventStatus.Id });
        }

        [HttpPost("group/item-type")]
        public async Task<ActionResult> CreateGroupItemType([FromBody] GroupItemTypeCreateDTO groupItemType)
        {
            var _groupItemType = _mapper.Map<GroupItemType>(groupItemType);
            await _context.GroupItemTypes.AddAsync(_groupItemType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGroupItemType), new { id = _groupItemType.Id });
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
                    return BadRequest(new
                    {
                        error = $"The platform type with id: {social.PlatformTypeId} don't exists"
                    });
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
