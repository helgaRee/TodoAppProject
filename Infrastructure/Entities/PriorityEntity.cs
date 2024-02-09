using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class PriorityEntity
{
    [Key]
    [Column(TypeName = "int")]
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string PriorityLevel { get; set; } = null!;

    public virtual ICollection<TaskEntity> Tasks { get; set; } = new HashSet<TaskEntity>();

}
