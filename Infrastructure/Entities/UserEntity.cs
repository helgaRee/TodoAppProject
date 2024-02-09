using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class UserEntity
{
    [Key]
    [Column(TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string UserName { get; set; } = null!;

    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Email { get; set; } = null!;

    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public string Password { get; set; } = null!;

    public virtual ICollection<TaskEntity> Tasks { get; set; } = new HashSet<TaskEntity>();
}
