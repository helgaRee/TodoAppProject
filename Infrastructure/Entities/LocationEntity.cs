using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Entities;

public class LocationEntity
{
    [Key]
    [Column(TypeName = "int")]
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(100)")]
    public string? LocationName { get; set; }

    [Column(TypeName = "nvarchar(100)")]
    public string? StreetName { get; set; }

    [Column(TypeName = "nvarchar(50)")]
    public string? City { get; set; }

    [Column(TypeName = "varchar(6)")]
    public string? PostalCode { get; set; }

    public virtual ICollection<TaskEntity> Tasks { get; set; } = new HashSet<TaskEntity>();

}
