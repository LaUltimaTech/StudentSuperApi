using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentSuperApi.Models;
[Table("tbl_states")]    
public class State
{
    [Key]
    public int StateID { get; set; }
    public string StateName { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<StateCity>? Cities { get; set; }
    public ICollection<SchoolBasicInformation>? Schools { get; set; }
}
