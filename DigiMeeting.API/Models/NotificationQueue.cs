namespace DigiMeeting.API.Models;

public class NotificationQueue
{
    public int Id { get; set; }
    public string RecipientTeamName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsProcessed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
}
