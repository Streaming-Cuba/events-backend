using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.Models {
    public abstract class BaseModel
    {
        public virtual bool EnsureValidState (ModelStateDictionary ModelState) => true;

        // public DateTime CreatedAt { get; set; }

        // public DateTime ModifiedAt { get; set; }
    }
}