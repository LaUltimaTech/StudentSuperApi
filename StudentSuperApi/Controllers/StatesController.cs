using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudentSuperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public StatesController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetStates()
        {
            var states = await _context.States.ToListAsync();
            return Ok(states);
        }
    }
}
