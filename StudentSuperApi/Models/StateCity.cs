using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuperApi.Models;
[Table("tbl_state_cities")]
public class StateCity
{
    [Key]
    public int CityID { get; set; }
    public int StateID { get; set; }
    public string CityName { get; set; } = string.Empty;

    // Navigation properties
    public State? State { get; set; }
    public ICollection<SchoolBasicInformation>? Schools { get; set; }
}
