using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuperApi.Models;

namespace StudentSuperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SchoolsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SchoolsController(ApplicationDbContext context) => _context = context;

        [HttpGet]
        public async Task<IActionResult> GetSchools()
        {
            var schools = await _context.Schools
                .Include(s => s.State)
                .Include(s => s.City)
                .ToListAsync();
            return Ok(schools);
        }
        [HttpPost]
        public async Task<IActionResult> AddSchool([FromBody] SchoolBasicInformation school)
        {
            school.State = null;
            school.City = null;
            school.Admins = null;

            _context.Schools.Add(school);
            await _context.SaveChangesAsync();
            return Ok(school);
        }

        


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchool(int id, [FromBody] SchoolBasicInformation school)
        {
            var existing = await _context.Schools.FindAsync(id);
            if (existing == null) return NotFound();

            existing.SchoolName = school.SchoolName;
            existing.SchoolAddress = school.SchoolAddress;
            existing.State_Id_fk = school.State_Id_fk;
            existing.City_Id_fk = school.City_Id_fk;
            existing.Email = school.Email;
            existing.MobileNo = school.MobileNo;
            existing.Website = school.Website;
            existing.BoardName = school.BoardName;
            existing.EstablishmentYear = school.EstablishmentYear;
            existing.SchoolInformation = school.SchoolInformation;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchool(int id)
        {
            var school = await _context.Schools.FindAsync(id);
            if (school == null) return NotFound();

            _context.Schools.Remove(school);
            await _context.SaveChangesAsync();
            return Ok();
        }


        [HttpGet("Names")]
        public async Task<IActionResult> GetSchoolNames()
        {
            var schools = await _context.Schools
                .Select(s => new { s.School_Id_pk, s.SchoolName })
                .ToListAsync();
            return Ok(schools);
        }
    }

}
