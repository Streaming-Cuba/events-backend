using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Events.API.DTO
{
    public class SocialCreateDTO
    {
        public int? PlatformTypeId { get; set; }

        [Required]
        public string Url { get; set; }
    }

    public class NTagCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class GroupItemTypeCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class NEventStatusCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class InteractionCreateDTO
    {
        public bool? Like { get; set; }

        public bool? Love { get; set; }
    }

    public class NCategoryCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class SocialPlatformTypeCreateDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class EventCreateDTO
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
    }
}