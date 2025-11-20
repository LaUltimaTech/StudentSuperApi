using Microsoft.EntityFrameworkCore.Metadata.Internal;
using StudentSuperApi.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuperApi.Models
{
    [Table("tbl_school_admin_info")]
    public class SchoolAdminInfo
    {
        [Key]
        public int Admin_Id_pk { get; set; }
        public string AdminName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public int School_Id_pk { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Admin_ID { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedBySuperAdmin { get; set; }

        // Navigation properties
        public SchoolBasicInformation? School { get; set; }
        public SuperAdminCredential? SuperAdmin { get; set; }

        // ✅ Add this so we can receive image file but NOT store it in DB
        [NotMapped]
        public IFormFile? AdminProfilePicFile { get; set; }
    }
}
