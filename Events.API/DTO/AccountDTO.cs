using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Events.API.Helpers;
using Events.API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace Events.API.DTO
{
    public class AccountCreateDTO : CreateModelDTO
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string AvatarPath { get; set; }

        [Required]
        public ICollection<int> RolesId { get; set; }

        [Required]
        public string Password { get; set; }

        public override async Task<bool> EnsureValidStateOnPatch(DbContext context, ModelStateDictionary ModelState)
        {
            if (!Email.IsEmail())
            {
                ModelState.AddModelError($"{nameof(Account.Email)}", "The email isn't valid");
                return false;
            }

            if ((!string.IsNullOrWhiteSpace(AvatarPath) && !AvatarPath.IsUrl()))
            {
                ModelState.AddModelError($"{nameof(Account.AvatarPath)}", "The avatar path must be a valid url");
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError($"{nameof(Account.Password)}", "The password must be no null or empty");
                return false;
            }

            // if ((await context.Set<Account>().AnyAsync(x => x.Email == Email)))
            // {
            //     ModelState.AddModelError($"{nameof(Account.Email)}", "Already exists a account with this email");
            //     return false;
            // }

            if (RolesId != null)
            {
                foreach (var roleId in RolesId)
                {
                    if (!(await context.Set<Role>().AnyAsync(x => x.Id == roleId)))
                    {
                        ModelState.AddModelError($"{nameof(Account.Roles)}", $"The role with id: {roleId} don't exists");
                        return false;
                    }
                }
            }
            else
            {
                ModelState.AddModelError($"{nameof(Account.Roles)}", $"The account must have least one role");
                return false;
            }

            return await base.EnsureValidStateOnPatch(context, ModelState);
        }
    }

    public class AccountReadDTO
    {
        public string Name { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AvatarPath { get; set; }

        public ICollection<int> RolesId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }

    public class RoleCreateDTO : CreateModelDTO
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public override async Task<bool> EnsureValidStateOnCreate(DbContext context, ModelStateDictionary ModelState)
        {
            if ((await context.Set<Role>().AnyAsync(x => x.Name == Name)))
            {
                ModelState.AddModelError($"{nameof(Role.Name)}", "Already exists a role with this name");
                return false;
            }

            return await base.EnsureValidStateOnCreate(context, ModelState);
        }
    }
}