using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Events.API.DTO
{
    public class SubscriberCreateDTO
    {
        public string Name { get; set; }

        public string Institution { get; set; }

        [Required]
        public string Email { get; set; }
    }
}