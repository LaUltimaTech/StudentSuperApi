using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSuperApi.Models;

namespace StudentSuperApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET ALL ADMINS
        [HttpGet]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _context.SchoolAdmins
                .Include(a => a.School)
                .Select(a => new
                {
                    a.AdminID,
                    a.AdminName,
                    a.Username,
                    a.Admin_ID,
                    a.PhotoPath,
                    SchoolName = a.School.SchoolName
                })
                .ToListAsync();

            return Ok(admins);
        }

        // GET ADMIN BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdmin(int id)
        {
            var admin = await _context.SchoolAdmins
                .Include(a => a.School)
                .FirstOrDefaultAsync(a => a.AdminID == id);

            if (admin == null) return NotFound();

            return Ok(admin);
        }

        // ADD ADMIN WITH IMAGE UPLOAD
        [HttpPost]
        public async Task<IActionResult> AddAdmin([FromForm] SchoolAdminInfo admin)
        {
            // Save image
            if (admin.AdminProfilePicFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "AdminPhotos");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(admin.AdminProfilePicFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await admin.AdminProfilePicFile.CopyToAsync(stream);
                }

                // Save only relative path in DB
                admin.PhotoPath = "/AdminPhotos/" + fileName;
            }

            admin.CreatedDate = DateTime.Now;
            admin.SuperAdmin = null;
            admin.School = null;

            _context.SchoolAdmins.Add(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin Added Successfully", admin });
        }

        // UPDATE ADMIN WITH IMAGE OPTIONAL
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromForm] SchoolAdminInfo admin)
        {
            var existing = await _context.SchoolAdmins.FindAsync(id);
            if (existing == null) return NotFound();

            existing.AdminName = admin.AdminName;
            existing.DateOfBirth = admin.DateOfBirth;
            existing.School_Id_fk = admin.School_Id_fk;
            existing.Username = admin.Username;
            existing.Password = admin.Password;
            existing.Admin_ID = admin.Admin_ID;

            // If new image uploaded, replace the old one
            if (admin.AdminProfilePicFile != null)
            {
                string folder = Path.Combine(_env.WebRootPath, "AdminPhotos");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(admin.AdminProfilePicFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await admin.AdminProfilePicFile.CopyToAsync(stream);
                }

                existing.PhotoPath = "/AdminPhotos/" + fileName;
            }

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // DELETE ADMIN
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.SchoolAdmins.FindAsync(id);
            if (admin == null) return NotFound();

            _context.SchoolAdmins.Remove(admin);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin deleted successfully" });
        }
    }
}
