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
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Events.API.Helpers;

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
        public async Task<ActionResult<IEnumerable<Event>>> ListEvents() => Ok(await _context.Events.ToListAsync());

        [HttpGet("{identifier}")]
        public async Task<ActionResult<Event>> EventByIdentifier([FromRoute] string identifier)
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
                    .AsSplitQuery() // perform in multiples queries

                    .SingleOrDefaultAsync(x => x.Identifier == identifier);

            if (item == null)
                return NotFound();

            // bad performance
            if (item.Groups != null)
                item.Groups = item.Groups.OrderBy(x => x.Order).ToList();

            return Ok(item);
        }

        [HttpGet("{identifier}/votes")]
        public ActionResult VotesByIdentifier([FromRoute] string identifier, [FromQuery] string type, [FromQuery] int? limit)
        {
            // [WARNING]: Super worst performance           
            var result = _context.GroupItemVotes.Include(d => d.GroupItem)
                                                .ThenInclude(p => p.Group)
                                                .ThenInclude(p => p.Event)

                                                .Include(d => d.GroupItem)
                                                .ThenInclude(p => p.Group)
                                                .ThenInclude(p => p.GroupParent)
                                                .ThenInclude(p => p.Event)

                                                .Include(d => d.GroupItem)
                                                .ThenInclude(p => p.Group)
                                                .ThenInclude(p => p.GroupParent)

                                                .Include(d => d.GroupItem)
                                                .ThenInclude(p => p.Metadata)

                                                .AsSplitQuery() // perform in multiples queries                                                
                                                .AsEnumerable()
                                                .Where(x =>
                                                {
                                                    Group p = x.GroupItem.Group;
                                                    while (p.EventId == null)
                                                        p = p.GroupParent;
                                                    return p.Event.Identifier == identifier;
                                                })
                                                .OrderByDescending(x => x.Count)
                                                .AsEnumerable();

            if (result != null)
            {
                if (!string.IsNullOrWhiteSpace(type))
                    result = result.Where(x => x.Type == type);

                if (limit != null)
                    result = result.Take(limit.Value);

                return Ok(result.Select(x => new
                {
                    id = x.Id,
                    count = x.Count,
                    type = x.Type,
                    groupItemName = x.GroupItem.Name,
                    groupItemCoverPath = x.GroupItem.CoverPath,
                    interpreter = x.GroupItem.Metadata?.Interpreter,
                    productor = x.GroupItem?.Metadata.Productor
                }).ToList());
            }
            else
            {
                // TODO: Replace
                return Ok(new GroupItemVote[0]);
            }
        }
        #endregion

        #region Create models and push to database
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<ActionResult> CreateEvent([FromBody] EventCreateDTO @event)
        {
            // validate data
            var _event = _mapper.Map<Event>(@event);

            if (@event.CategoryId != null)
            {
                _event.Category = await _context.Categories.FindAsync(@event.CategoryId);
                if (_event.Category == null)
                    return BadRequest(new
                    {
                        error = $"The category with id: {@event.CategoryId} don't exists"
                    });
            }

            _event.Status = await _context.EventStatuses.FindAsync(@event.StatusId);
            if (_event.Status == null)
                return BadRequest(new
                {
                    error = $"The status with id: {@event.StatusId} don't exists"
                });

            if (@event.TagsId != null)
            {
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
            }

            _event.CreatedAt = DateTime.UtcNow;
            _event.ModifiedAt = DateTime.UtcNow;

            return CreatedAtAction(nameof(CreateEvent), new { id = _event.Id });
        }

        // [HttpPost("interaction/{eventId}")]
        // public async Task<ActionResult> CreateInteraction([FromBody] InteractionCreateDTO interaction) 
        // {

        // }

        [Authorize(Roles = "Administrator")]
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
            _groupItem.Group = _group;
            _groupItem.GroupId = _group.Id; // drop

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("vote")]
        public async Task<ActionResult> CreateVote([FromBody] GroupItemVoteCreateDTO vote)
        {
            var groupItem = await _context.GroupItems.Include(d => d.Votes)
                                                     .FirstOrDefaultAsync(x => x.Id == vote.GroupItemId);
            if (groupItem == null)
                return BadRequest(new
                {
                    error = $"The group item with id: {vote.GroupItemId} don't exists"
                });

            var _vote = groupItem.Votes.FirstOrDefault(x => x.Type == vote.Type);

            if (_vote == null)
            {
                _vote = new GroupItemVote
                {
                    Count = 1,
                    Type = vote.Type
                };

                groupItem.Votes.Add(_vote);
            }
            else
            {
                _vote.Count++;
            }

            await _context.SaveChangesAsync();
            return Ok(new
            {
                groupItemId = vote.GroupItemId,
                groupId = groupItem.GroupId,
            });
        }

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPost("tag")]
        public async Task<ActionResult> CreateTag([FromBody] NTagCreateDTO tag)
        {
            if ((await _context.Tags.FirstOrDefaultAsync(x => x.Name == tag.Name))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a tag with this name"
                });

            var _tag = _mapper.Map<NTag>(tag);
            await _context.Tags.AddAsync(_tag);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateTag), new { id = _tag.Id });
        }

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPost("social/platform-type")]
        public async Task<ActionResult> CreateSocialPlatformType([FromBody] SocialPlatformTypeCreateDTO socialPlatformType)
        {
            if ((await _context.SocialPlatformTypes.FirstOrDefaultAsync(x => x.Name == socialPlatformType.Name))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a social platform type with this name"
                });

            var _socialPlatformType = _mapper.Map<SocialPlatformType>(socialPlatformType);
            await _context.SocialPlatformTypes.AddAsync(_socialPlatformType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocialPlatformType), new { id = _socialPlatformType.Id });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("category")]
        public async Task<ActionResult> CreateCategory([FromBody] NCategoryCreateDTO category)
        {
            if ((await _context.Categories.FirstOrDefaultAsync(x => x.Name == category.Name))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a tag with this name"
                });

            var _category = _mapper.Map<NCategory>(category);
            await _context.Categories.AddAsync(_category);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateSocial), new { id = _category.Id });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("event-status")]
        public async Task<ActionResult> CreateEventStatus([FromBody] NEventStatusCreateDTO eventStatus)
        {
            if ((await _context.EventStatuses.FirstOrDefaultAsync(x => x.Name == eventStatus.Name))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a event status with this name"
                });

            var _eventStatus = _mapper.Map<NEventStatus>(eventStatus);
            await _context.EventStatuses.AddAsync(_eventStatus);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateEventStatus), new { id = _eventStatus.Id });
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost("group/item-type")]
        public async Task<ActionResult> CreateGroupItemType([FromBody] GroupItemTypeCreateDTO groupItemType)
        {
            if ((await _context.GroupItemTypes.FirstOrDefaultAsync(x => x.Name == groupItemType.Name))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a tag with this name"
                });

            var _groupItemType = _mapper.Map<GroupItemType>(groupItemType);
            await _context.GroupItemTypes.AddAsync(_groupItemType);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(CreateGroupItemType), new { id = _groupItemType.Id });
        }
        #endregion

        #region Modify models 
        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
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

        #region Patch models
        [Authorize(Roles = "Administrator")]
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchEvent([FromRoute] int id,
                                                   [FromBody] JsonPatchDocument<EventCreateDTO> @event)
        {
            // validate data
            var _event = await _context.Events.Include(d => d.Tags)
                                              .Include(d => d.Category)
                                              .Include(d => d.Status)
                                              .FirstOrDefaultAsync(x => x.Id == id);
            if (_event == null)
                return NotFound(id);

            // [TODO]: Only check changed values
            await @event.ApplyTo(_event, _context, _mapper, ModelState, s =>
            {
                // Manual sync with EventsTags
                if (s.TagsId != null)
                {
                    foreach (var tagId in s.TagsId)
                    {
                        if (!_event.Tags.Any(x => x.TagId == tagId))
                            _event.Tags.Add(new EventTag
                            {
                                TagId = tagId,
                                EventId = id
                            });
                    }
                    var toRemove = _event.Tags.Where(x => !s.TagsId.Contains(x.TagId)).ToList();
                    foreach (var item in toRemove)
                        _event.Tags.Remove(item);
                }
            });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _event.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("group/item-type/{id}")]
        public async Task<ActionResult> PatchGroupItemType([FromRoute] int id,
                                                           [FromBody] JsonPatchDocument<GroupItemTypeCreateDTO> groupItemType)
        {
            var _groupItemType = await _context.GroupItemTypes.FindAsync(id);
            if (_groupItemType == null)
                return NotFound(id);

            await groupItemType.ApplyTo(_groupItemType, _context, _mapper, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("social/{id}")]
        public async Task<ActionResult> PatchSocial([FromRoute] int id,
                                                    [FromBody] JsonPatchDocument<SocialCreateDTO> social)
        {
            var _social = await _context.Socials.FindAsync(id);
            if (_social == null)
                return NotFound(id);

            await social.ApplyTo(_social, _context, _mapper, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("event-status/{id}")]
        public async Task<ActionResult> PatchEventStatus([FromRoute] int id,
                                                         [FromBody] JsonPatchDocument<NEventStatusCreateDTO> eventStatus)
        {
            var _eventStatus = await _context.EventStatuses.FindAsync(id);
            if (_eventStatus == null)
                return NotFound(id);

            await eventStatus.ApplyTo(_eventStatus, _context, _mapper, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [Authorize(Roles = "Administrator")]
        [HttpPatch("interaction/{id}")]
        public async Task<ActionResult> PatchInteraction([FromRoute] int id,
                                                         [FromBody] JsonPatchDocument<InteractionCreateDTO> interaction)
        {
            var _interaction = await _context.Interactions.FindAsync(id);
            if (_interaction == null)
                return NotFound(id);

            await interaction.ApplyTo(_interaction, _context, _mapper, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("tag/{id}")]
        public async Task<ActionResult> PatchTag([FromRoute] int id,
                                                 [FromBody] JsonPatchDocument<NTagCreateDTO> tag)
        {
            var _tag = await _context.Tags.FindAsync(id);
            if (_tag == null)
                return NotFound(id);

            await tag.ApplyTo(_tag, _context, _mapper, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("category/{id}")]
        public async Task<ActionResult> PatchCategory([FromRoute] int id,
                                                      [FromBody] JsonPatchDocument<NCategoryCreateDTO> category)
        {
            var _category = await _context.Categories.FindAsync(id);
            if (_category == null)
                return NotFound(id);

            await category.ApplyTo(_category, _context, _mapper, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [Authorize(Roles = "Administrator")]
        [HttpPatch("social/platform-type/{id}")]
        public async Task<ActionResult> PatchSocialPlatformType([FromRoute] int id,
                                                                [FromBody] JsonPatchDocument<SocialPlatformTypeCreateDTO> socialPlatformType)
        {
            var _socialPlatformType = await _context.Tags.FindAsync(id);
            if (_socialPlatformType == null)
                return NotFound(id);

            await socialPlatformType.ApplyTo(_socialPlatformType, _context, _mapper, ModelState);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            await _context.SaveChangesAsync();
            return Ok();
        }
        #endregion
    }
}
