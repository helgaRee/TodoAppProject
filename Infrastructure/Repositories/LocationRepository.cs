using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class LocationRepository(DataContext context) : BaseRepository<LocationEntity>(context)
{
    private readonly DataContext _context = context;

    /// <summary>
    /// This method gets a specific location from the database and includes tasks related to it.
    /// </summary>
    /// <param name="expression">Uses an expression to search.</param>
    /// <returns>Returns a locationEntity if exists, else null.</returns>
    public override async Task<LocationEntity> GetAsync(Expression<Func<LocationEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Locations
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
    /// Gets all locations from the database, includes related tasks for each location.
    /// </summary>
    /// <returns>Returns existing locationentities in a list, else null.</returns>
    public override async Task<IEnumerable<LocationEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Locations
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
