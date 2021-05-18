using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models {
    public abstract class BaseModel
    {
        // public DateTime CreatedAt { get; set; }

        // public DateTime ModifiedAt { get; set; }
    }
}