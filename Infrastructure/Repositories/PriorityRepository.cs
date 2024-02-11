using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class PriorityRepository(DataContext context) : BaseRepository<PriorityEntity>(context)
{
    private readonly DataContext _context = context;

    /// <summary>
    /// Gets a specific priority from the database, includes related tasks.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>Returns a priorityEntity, else null.</returns>
    public override async Task<PriorityEntity> GetAsync(Expression<Func<PriorityEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Priorities
                .Include(i => i.Tasks)
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
    /// Gets all priorityEntities in the database, includes all related tasks for each priority.
    /// </summary>
    /// <returns>Returns PriorityEntities as a list, else an empty list.</returns>
    public override async Task<IEnumerable<PriorityEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Priorities
                .Include(i => i.Tasks)
                .ToListAsync();

            if (existingEntities != null)
            {
                return existingEntities;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }
}
