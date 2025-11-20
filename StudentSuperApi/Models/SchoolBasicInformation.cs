using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace StudentSuperApi.Models;

[Table("tbl_school_basic_information")]
public class SchoolBasicInformation
{
    [Key]
    public int SchoolID { get; set; }
    public string SchoolName { get; set; } = string.Empty;
    public string? SchoolAddress { get; set; }
    public int StateID { get; set; }
    public int CityID { get; set; }
    public string? Email { get; set; }
    public string? MobileNo { get; set; }
    public string? Website { get; set; }
    public string? BoardName { get; set; }
    public string SchoolCode { get; set; } = string.Empty;
    public int? EstablishmentYear { get; set; }
    public string? SchoolInformation { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [JsonIgnore]
    public State? State { get; set; }

    [JsonIgnore]
    public StateCity? City { get; set; }

    [JsonIgnore]
    public ICollection<SchoolAdminInfo>? Admins { get; set; }
}
