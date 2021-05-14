using System;
using Xunit;
using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

using Events.API.Models;
using Events.API.Controllers;
using Events.API.Profiles;
using Events.API.Data;
using Events.API.DTO;
using System.Threading;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Tests
{
    public class AccountControllerTest
    {
        (AccountController, AccountContext) CreateContextForAddition()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AccountProfile>();
            });
            var mapper = new Mapper(config);
            var options = new DbContextOptionsBuilder<AccountContext>()
                .UseInMemoryDatabase(databaseName: $"Test{Guid.NewGuid()}").Options;

            var context = new AccountContext(options);
            return (new AccountController(context, mapper), context);
        }

        [Fact]
        public async Task Simple_CreateRoleTest()
        {
            (var c, var context) = CreateContextForAddition();
            var result = await c.CreateRole(new RoleCreateDTO
            {
                Name = "Administrator",
                Description = "Don't work any time"
            });

            // Asserts
            Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(context.Roles.Count(), 1);
            var role = await context.Roles.FirstOrDefaultAsync();

            Assert.NotNull(role);
            Assert.Equal(role.Id, 1);
            Assert.Equal(role.Name, "Administrator");
            Assert.Equal(role.Description, "Don't work any time");
        }

        [Fact]
        public async Task DobleInsertion_CreateRoleTest()
        {
            (var c, var context) = CreateContextForAddition();
            await c.CreateRole(new RoleCreateDTO
            {
                Name = "Administrator",
                Description = "Don't work any time"
            });

            var result = await c.CreateRole(new RoleCreateDTO
            {
                Name = "Administrator",
                Description = "Don't work any time"
            });

            // Asserts
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Simple_CreateAccountTest()
        {
            (var c, var context) = CreateContextForAddition();
            await c.CreateRole(new RoleCreateDTO
            {
                Name = "Administrator",
                Description = "Don't work any time"
            });

            var role = await context.Roles.FirstOrDefaultAsync();

            var result = await c.CreateAccount(new AccountCreateDTO
            {
                Name = "Juan",
                LastName = "Cabilla Gonzales",
                AvatarPath = "https://github.com/cl8dep.png",
                Email = "cl8dep@gmail.com",
                Password = "12345678",
                RolesId = new List<int> { 1 },
            });

            // Asserts
            Assert.IsType<CreatedAtActionResult>(result);

            Assert.Equal(context.Accounts.Count(), 1);
            var account = await context.Accounts.Include(d => d.Roles).FirstOrDefaultAsync();

            Assert.NotNull(account);
            Assert.Equal(account.Id, 1);
            Assert.Equal(account.Name, "Juan");
            Assert.Equal(account.LastName, "Cabilla Gonzales");
            Assert.Equal(account.AvatarPath, "https://github.com/cl8dep.png");
            Assert.Equal(account.Email, "cl8dep@gmail.com");
            Assert.NotEqual(account.Password, "12345678");
            Assert.Equal(account.Roles.Count, 1);
            Assert.Equal(account.Roles.FirstOrDefault().Role, role);
        }

        [Fact]
        public async Task DobleInsertion_CreateAccountTest()
        {
            (var c, var context) = CreateContextForAddition();
            await c.CreateRole(Mock.Of<RoleCreateDTO>());

            await c.CreateAccount(new AccountCreateDTO
            {
                Email = "cl8dep@gmail.com",
                Password = "12345678",
                RolesId = new List<int> { 1 },
            });

            var result = await c.CreateAccount(new AccountCreateDTO
            {
                Email = "cl8dep@gmail.com",
                Password = "1a1a1a1a1a1a",
                RolesId = new List<int> { 1, 2 },
            });

            // Asserts
            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}