using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.DTO
{
    public abstract class CreateModelDTO
    {
        public virtual async Task<bool> EnsureValidStateOnPatch (DbContext context,
                                                                 ModelStateDictionary ModelState) => await Task.FromResult(true);

        public virtual async Task<bool> EnsureValidStateOnCreate (DbContext context,
                                                                  ModelStateDictionary ModelState) => await Task.FromResult(true);
    }
}