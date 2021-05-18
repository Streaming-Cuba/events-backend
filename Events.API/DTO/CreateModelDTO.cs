using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.DTO
{
    public abstract class CreateModelDTO
    {
        public virtual async Task<bool> EnsureValidState (DbContext context,
                                                          ModelStateDictionary ModelState) => true;
    }
}