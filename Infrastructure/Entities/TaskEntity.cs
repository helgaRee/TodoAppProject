using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class TaskEntity
{
    [Key]
    [Column(TypeName = "int")]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string Title { get; set; } = null!;

    [Column(TypeName = "nvarchar(450)")]
    public string? Description { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string? Deadline { get; set; }

    [Column(TypeName = "nvarchar(20)")]
    public string? Status { get; set; }



    public int? LocationId { get; set; }
    public virtual LocationEntity? Location { get; set; } = null!;

    public int PriorityId { get; set; }
    public virtual PriorityEntity? Priority { get; set; } = null!;


    [Required]
    public Guid UserId { get; set; }
    public virtual UserEntity User { get; set; } = null!;

    [Required]
    public int CategoryId { get; set; }
    public virtual CategoryEntity Category { get; set; } = null!;
}

