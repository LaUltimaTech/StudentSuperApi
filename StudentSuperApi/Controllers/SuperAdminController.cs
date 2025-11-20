using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuperApi.Models;
using System.Threading.Tasks;

namespace StudentSuperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SuperAdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/SuperAdmin/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password are required.");

            var superAdmin = await _context.SuperAdmins
                .FirstOrDefaultAsync(sa => sa.UserName == request.UserName && sa.IsActive);

            if (superAdmin == null)
                return Unauthorized("Invalid username or inactive account.");

            // ⚠️ Use hashing in real systems
            if (superAdmin.Password != request.Password)
                return Unauthorized("Invalid password.");

            return Ok(new
            {
                Message = "Login successful",
                SuperAdminID = superAdmin.SuperAdminID,
                UserName = superAdmin.UserName
            });
        }
    }

    // DTO for login request
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
