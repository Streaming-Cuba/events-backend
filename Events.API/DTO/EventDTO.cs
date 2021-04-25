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
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class GroupItemTypeCreateDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class NEventStatusCreateDTO
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class InteractionCreateDTO
    {
        public bool? Like { get; set; }

        public bool? Love { get; set; }
    }
}