using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure;

public class CategoryEntity
{
    [Key]
    [Column(TypeName = "int")]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string CategoryName { get; set; } = null!;

    [Column(TypeName = "bit")]
    public bool? IsPrivate { get; set; } = false;

    public virtual ICollection<TaskEntity> Tasks { get; set; } = new HashSet<TaskEntity>();
}
