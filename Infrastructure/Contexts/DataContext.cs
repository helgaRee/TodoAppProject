using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public virtual DbSet<PriorityEntity> Priorities { get; set; }
    public virtual DbSet<CategoryEntity> Categories { get; set; }
    public virtual DbSet<LocationEntity> Locations { get; set; }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<TaskEntity> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.UserName)
            .IsUnique();
        modelBuilder.Entity<UserEntity>()
            .HasIndex(x => x.Email)
            .IsUnique();
    }
}
