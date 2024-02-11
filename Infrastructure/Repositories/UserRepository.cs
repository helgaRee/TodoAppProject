using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public class UserRepository(DataContext context) : BaseRepository<UserEntity>(context)
{
    private readonly DataContext _context = context;

    /// <summary>
    /// Gets a userEntity from the database, and includes the tasks related to the user.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>Returns a userEntity if exist, else null.</returns>
    public override async Task<UserEntity> GetAsync(Expression<Func<UserEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Users
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
    /// Gets all userEntities in the database and includes all the tasks realted to them.
    /// </summary>
    /// <returns>Returns a list of userEntities in the database, else null.</returns>
    public override async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Users
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
