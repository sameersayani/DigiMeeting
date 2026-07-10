namespace DigiMeeting.API.DTOs;

public class BookingRequest
{
    public int TeamId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
