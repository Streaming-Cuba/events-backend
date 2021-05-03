using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Events.API.Data;
using Events.API.DTO;
using Events.API.Helpers;
using Events.API.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Events.API.Controllers
{
    [Route("/api/v1/subscriber")]
    [ApiController]
    public class SubscriberController : ControllerBase
    {
        private readonly SubscriberContext _context;
        private readonly IMapper _mapper;

        public SubscriberController(SubscriberContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] SubscriberCreateDTO subscriber)
        {
            if (!subscriber.Email.IsEmail())
                return ValidationProblem();
            if ((await _context.Subscribers.FirstOrDefaultAsync(x => x.Email == subscriber.Email))
                != null)
                return BadRequest(new
                {
                    error = "Already exists a subscriber with this email"
                });

            var _subscriber = _mapper.Map<Subscriber>(subscriber);
            await _context.Subscribers.AddAsync(_subscriber);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Create), new { id = _subscriber.Id });
        }
    }
}
