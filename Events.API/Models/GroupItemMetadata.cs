using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models
{
    public class GroupItemMetadata
    {
        [Key]
        public int Id { get; set; }

        public string ProductorHome { get; set; }

        public string Interpreter { get; set; }

        public string Productor { get; set; }

        public string Support { get; set; }

        public string Url { get; set; }
    }
}