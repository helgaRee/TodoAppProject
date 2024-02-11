using Infrastructure.Contexts;
using Infrastructure.Dtos;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;
public class TaskRepository(DataContext context) : BaseRepository<TaskEntity>(context)
{
    private readonly DataContext _context = context;

    /// <summary>
    /// Gets a TaskEntity from the database, which includes the entites Location, Priority, User and Category realted to the task.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>A taskentity, else null.</returns>
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

    /// <summary>
    /// Gets all the taskEntities from the database.
    /// </summary>
    /// <returns>A list of taskentities with included information, else null.</returns>
    public override async Task<IEnumerable<TaskEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Tasks
                .Include(i => i.Location)
                .Include(i => i.Priority)
                .Include(i => i.User)
                .Include(i => i.Category)
                .ToListAsync();

            if (existingEntities != null)
            {
                return existingEntities;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    /// <summary>
    /// This method converts a TaskEntity to a TaskDto. 
    /// </summary>
    /// <param name="entity"></param>
    /// <returns>A new TaskDto with the same values as the entity.</returns>
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

    /// <summary>
    /// Gets all tasks related to a given category from database.This includes information about category for each task.
    /// </summary>
    /// <param name="categoryName"></param>
    /// <returns>Returns a list with the existing tasks related to a specific category, else an empty list.</returns>
    public async Task<IEnumerable<TaskEntity>> GetTasksForCategoryAsync(string categoryName)
    {
        try
        {
            var tasks = await _context.Tasks
                .Include(task => task.Category)
                .Where(task => task.Category.CategoryName == categoryName)
                .ToListAsync();
            return tasks;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return Enumerable.Empty<TaskEntity>();
        }
    }

    /// <summary>
    /// Gets all the tasks related to a specific userin database. Includes userinformation for each task.
    /// </summary>
    /// <param name="userName"></param>
    /// <returns>Returns a list of tasks in the database related to the given user. Else returns an empty list.</returns>
    public async Task<IEnumerable<TaskEntity>> GetTasksForUserAsync(string userName)
    {
        try
        {
            var tasks = await _context.Tasks
                .Include(task => task.User) 
                .Where(task => task.User.UserName == userName)
                .ToListAsync();
            return tasks;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return Enumerable.Empty<TaskEntity>();
        }
    }
}
