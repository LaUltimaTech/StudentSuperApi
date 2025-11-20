using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuperApi.Models;

[Table("tbl_super_admin_crediential")]
public class SuperAdminCredential
{
    [Key]
    public int SuperAdminID { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<SchoolAdminInfo>? SchoolAdmins { get; set; }
}
