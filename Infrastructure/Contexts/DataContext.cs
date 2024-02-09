using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    //namnge här vad tabellerna ska heta i Databasen! jämför med dina modeleringtabeller
    public virtual DbSet<PriorityEntity> Priorities { get; set; }
    public virtual DbSet<CategoryEntity> Categories { get; set; }
    public virtual DbSet<LocationEntity> Locations { get; set; }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //specificerar vilka attribut som är unika - se dina tabeller
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.UserName)
            .IsUnique();
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}
