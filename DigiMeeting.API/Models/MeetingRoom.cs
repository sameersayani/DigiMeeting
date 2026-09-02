public class MeetingRoom
{
    public int Id { get; set; }

    public required string Name { get; set; } = string.Empty;
    public required int Capacity { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
}