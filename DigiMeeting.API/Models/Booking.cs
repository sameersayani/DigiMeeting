namespace DigiMeeting.API.Models;

public class Booking
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsCancelled { get; set; } = false;
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties for EF Core relationships
    public Team? Team { get; set; }
    public MeetingRoom? Room { get; set; }
}