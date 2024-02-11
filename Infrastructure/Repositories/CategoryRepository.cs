using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class CategoryRepository(DataContext context) : BaseRepository<CategoryEntity>(context)
{
    private readonly DataContext _context = context;

    /// <summary>
    /// This method gets a specific category from the database.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>A categoryEntity, else null.</returns>
    public override async Task<CategoryEntity> GetAsync(Expression<Func<CategoryEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Categories
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
    /// A method to get all existing categoryEntities in the database. This method also includes tasks related to each category.
    /// </summary>
    /// <returns>Returns the existing categoryentities, else null.</returns>
    public override async Task<IEnumerable<CategoryEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Categories
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
