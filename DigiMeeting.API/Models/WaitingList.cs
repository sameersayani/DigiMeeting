namespace DigiMeeting.API.Models;

public class WaitingList
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public int RequiredCapacity { get; set; }
    public DateTime TargetStartTime { get; set; }
    public DateTime TargetEndTime { get; set; }
    
    // Status can be: "Active", "Fulfilled", "Expired", "TimedOut"
    public string Status { get; set; } = "Active"; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation property to join with Team details
    public Team? Team { get; set; }
}
