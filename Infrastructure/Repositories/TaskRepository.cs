using Infrastructure.Contexts;
using Infrastructure.Dtos;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;
using static Infrastructure.Repositories.TaskRepository;

namespace Infrastructure.Repositories;

public class TaskRepository(DataContext context) : BaseRepository<TaskEntity>(context)
{
    private readonly DataContext _context = context;

    public override async Task<TaskEntity> GetAsync(Expression<Func<TaskEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Tasks
                .Include(i => i.Location)
                .Include(i => i.Priority)
                .Include(i => i.User)
                .Include(i => i.Category)
                .FirstOrDefaultAsync(expression);

            if (existingEntity != null)
            {
                return existingEntity;
            }

        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return null!;
    }

    public override async Task<IEnumerable<TaskEntity>> GetAllAsync()
    {
        try
        {
            //hitta entiteten
            var existingEntities = await _context.Tasks
                .Include(i => i.Location)
                .Include(i => i.Priority)
                .Include(i => i.User)
                .Include(i => i.Category)
                .ToListAsync();
            //returnera om inte null
            if (existingEntities != null)
            {
                return existingEntities;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    public TaskDto MapToDto(TaskDto entity)
    {
        return new TaskDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Deadline = entity.Deadline,
            Status = entity.Status,
            CategoryName = entity.CategoryName,
            PriorityLevel = entity.PriorityLevel,
            LocationName = entity.LocationName,
            StreetName = entity.StreetName,
            PostalCode = entity.PostalCode,
            City = entity.City,
            UserName = entity.UserName,
            Email = entity.Email,
            Password = entity.Password,
            IsPrivate = entity.IsPrivate
        };
    }

}
