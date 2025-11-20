namespace StudentSuperApi.Models
{
    public class AdminCreateUpdateDto
    {
        public string Admin_ID { get; set; } = string.Empty;
        public string AdminName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public int SchoolID { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? PhotoPath { get; set; }
    }
}
