using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Events.API.Models
{
    public class EntryInfo
    {
        public string Name { get; set; }

        public string Extension { get; set; }

        public DateTime ModificationTime { get; set; }

        public bool IsDir { get; set; }
    }
}
