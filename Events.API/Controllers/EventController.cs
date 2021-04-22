using AutoMapper;
using Events.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace Events.API.Controllers
{
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly EventContext _context;
        private readonly IMapper _mapper;

        public EventController(EventContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        
    }
}
