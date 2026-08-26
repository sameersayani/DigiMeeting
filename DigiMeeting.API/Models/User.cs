public class User
{
    public int Id { get; set; }
    public required string Auth0Id { get; set; } // From Auth0
    public required string Email { get; set; }
    public required string Name { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
}