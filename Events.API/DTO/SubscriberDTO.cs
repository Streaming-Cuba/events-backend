using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Events.API.Models;

namespace Events.API.DTO
{
    public class SubscriberCreateDTO : BaseModel
    {
        public string Name { get; set; }

        public string Institution { get; set; }

        [Required]
        public string Email { get; set; }
    }
}