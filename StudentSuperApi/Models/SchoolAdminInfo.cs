using Microsoft.AspNetCore.Http;
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

        // Match DB column name and mark as FK for the School navigation
        [Column("School_Id_fk")]
        [ForeignKey(nameof(School))]
        public int School_Id_fk { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Admin_ID { get; set; }
        public string? PhotoPath { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? CreatedBySuperAdmin { get; set; }

        // Navigation properties
        public SchoolBasicInformation? School { get; set; }
        public SuperAdminCredential? SuperAdmin { get; set; }

        // Not stored in DB: used for receiving uploaded file
        [NotMapped]
        public IFormFile? AdminProfilePicFile { get; set; }
    }
}
