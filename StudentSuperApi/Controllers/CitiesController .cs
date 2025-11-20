using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace StudentSuperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CitiesController(ApplicationDbContext context) => _context = context;

        [HttpGet("{stateId}")]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var cities = await _context.StateCities
                .Where(c => c.State_Id_fk == stateId)
                .Where(c => c.StateID == stateId)
                .ToListAsync();
            return Ok(cities);
        }
    }

}
