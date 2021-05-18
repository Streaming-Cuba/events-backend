using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Events.API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.DTO
{
    public class GroupItemVoteCreateDTO : CreateModelDTO
    {
        [Required]
        public int GroupItemId { get; set; }

        [Required]
        public string Type { get; set; }
    }

    public class GroupCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class GroupItemCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public string CoverPath { get; set; }

        public uint Votes { get; set; }

        public GroupItemMetadataCreateDTO Metadata { get; set; }

        public int? TypeId { get; set; }
    }

    public class GroupItemMetadataCreateDTO : CreateModelDTO
    {
        public string ProductorHome { get; set; }

        public string Interpreter { get; set; }

        public string Productor { get; set; }

        public string Support { get; set; }

        public string Url { get; set; }
    }

    public class SocialCreateDTO : CreateModelDTO
    {
        public int? PlatformTypeId { get; set; }

        [Required]
        public string Url { get; set; }
    }

    public class NTagCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class GroupItemTypeCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class NEventStatusCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class InteractionCreateDTO : CreateModelDTO
    {
        public bool? Like { get; set; }

        public bool? Love { get; set; }
    }

    public class NCategoryCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class SocialPlatformTypeCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class EventCreateDTO : CreateModelDTO
    {
        [Required]
        public string Identifier { get; set; }

        [Required]
        public string Name { get; set; }

        public string Subtitle { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int StatusId { get; set; }

        public string Organizer { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        public ICollection<int> TagsId { get; set; }

        public string CoverPath { get; set; }

        public string ShortCoverPath { get; set; }

        public string Location { get; set; }

        public override async Task<bool> EnsureValidState(DbContext context, ModelStateDictionary ModelState)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError($"{nameof(Event.Name)}", "The name must be no null");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Identifier))
            {
                ModelState.AddModelError($"{nameof(Event.Identifier)}", "The identifier must be no null");
                return false;
            }

            if (!(await context.Set<NCategory>().AnyAsync(x => x.Id == CategoryId)))
            {
                ModelState.AddModelError($"{nameof(Event.CategoryId)}", $"The category with id: {CategoryId} don't exists");
                return false;
            }

            if (!(await context.Set<NEventStatus>().AnyAsync(x => x.Id == StatusId)))
            {
                ModelState.AddModelError($"{nameof(Event.StatusId)}", $"The status with id: {StatusId} don't exists");
                return false;
            }

            if (TagsId != null)
            {
                foreach (var tagId in TagsId)
                {
                    if (!(await context.Set<NTag>().AnyAsync(x => x.Id == tagId)))
                    {
                        ModelState.AddModelError($"{nameof(Event.Tags)}", $"The tag with id: {tagId} don't exists");
                        return false;
                    }
                }
            }

            return true;
        }
    }
}