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
        // (AccountController, AccountContext) CreateContextForAddition()
        // {
        //     var config = new MapperConfiguration(cfg =>
        //     {
        //         cfg.AddProfile<AccountProfile>();
        //     });
        //     var mapper = new Mapper(config);
        //     var options = new DbContextOptionsBuilder<AccountContext>()
        //         .UseInMemoryDatabase(databaseName: $"Test{Guid.NewGuid()}").Options;

        //     var context = new AccountContext(options);
        //     return (new AccountController(context, mapper), context);
        // }
    }
}
