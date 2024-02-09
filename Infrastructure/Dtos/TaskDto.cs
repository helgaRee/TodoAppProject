namespace Infrastructure.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? Deadline { get; set; }
    public string Status { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public bool IsPrivate { get; set; } = false!;

    public string PriorityLevel { get; set; } = null!;
    public string? LocationName { get; set; }
    public string? StreetName { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string UserName { get; set; } = null!;
    public string? Email { get; set; }
    public string? Password { get; set; }
}
