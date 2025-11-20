using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentSuperApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace StudentSuperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuperAdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtSettings _jwtSettings;

        public SuperAdminController(ApplicationDbContext context, JwtSettings jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings;
        }

        // POST: api/SuperAdmin/Login
        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Username and password are required.");

            var superAdmin = await _context.SuperAdmins
                .FirstOrDefaultAsync(sa => sa.UserName == request.UserName && sa.IsActive);

            if (superAdmin == null)
                return Unauthorized("Invalid username or inactive account.");

            // ⚠️ In production, store and verify hashed passwords
            if (superAdmin.Password != request.Password)
                return Unauthorized("Invalid password.");

            var key = _jwtSettings.Key;
            if (string.IsNullOrWhiteSpace(key))
                return StatusCode(500, "JWT key not configured on server.");

            var issuer = _jwtSettings.Issuer;
            var audience = _jwtSettings.Audience;
            var expiresMinutes = _jwtSettings.ExpiresMinutes > 0 ? _jwtSettings.ExpiresMinutes : 60;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, superAdmin.UserName),
                new Claim("SuperAdminId", superAdmin.SuperAdmin_Id_pk.ToString()),
                new Claim(ClaimTypes.Name, superAdmin.UserName),
                new Claim(ClaimTypes.Role, "SuperAdmin")
            };

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Message = "Login successful",
                Token = tokenString,
                ExpiresInMinutes = expiresMinutes,
                SuperAdminID = superAdmin.SuperAdmin_Id_pk,
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
