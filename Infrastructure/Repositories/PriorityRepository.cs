using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class PriorityRepository(DataContext context) : BaseRepository<PriorityEntity>(context)
{
    private readonly DataContext _context = context;

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
