namespace DigiMeeting.API.DTOs;

public class WaitlistRequestDto
{
    public int TeamId { get; set; }
    public DateTime TargetStartTime { get; set; }
    public DateTime TargetEndTime { get; set; }
}
