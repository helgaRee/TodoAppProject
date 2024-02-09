using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class LocationRepository(DataContext context) : BaseRepository<LocationEntity>(context)
{
    private readonly DataContext _context = context;

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

    public override async Task<IEnumerable<LocationEntity>> GetAllAsync()
    {
        try
        {
            //hitta entiteten
            var existingEntities = await _context.Locations
                .Include(i => i.Tasks)
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
}
