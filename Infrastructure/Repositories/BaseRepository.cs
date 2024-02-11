using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context;

    protected BaseRepository(DataContext context)
    {
        _context = context;
    }

/// <summary>
/// Adds a new entity to database.
/// </summary>
/// <param name="entity">Takes an entity and adds it to the context and saves changes to the database.</param>
/// <returns>Returns the new entity if succesfull, else null.</returns>
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        try
        {
            _context.Set<TEntity>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return null!;
    }


/// <summary>
/// Gets a specific entity from the database.
/// </summary>
/// <param name="expression"></param>
/// <returns>Returns the entity if exists, else null.</returns>
    public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Set<TEntity>().FirstOrDefaultAsync(expression);

            if (existingEntity != null)
            {
                return existingEntity;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return null!;
    }

/// <summary>
/// Gets all entities of type 'TEntity'
/// </summary>
/// <returns>Returns a list with the existing entities, else null.</returns>
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Set<TEntity>().ToListAsync();
            await _context.SaveChangesAsync();
            return existingEntities;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

/// <summary>
/// Updates an existing entity from the database.
/// </summary>
/// <param name="expression">Uses an expression to find the existing entity.</param>
/// <param name="updatedEntity">The updated entity</param>
/// <returns>Returns the updated entity, else null.</returns>
    public virtual async Task<TEntity> UpdateAsync(Expression<Func<TEntity, bool>> expression, TEntity updatedEntity)
    {
        try
        {
            var existingEntity = await _context.Set<TEntity>().FirstOrDefaultAsync(expression);
            if (existingEntity != null)
            {
                _context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
                await _context.SaveChangesAsync();
                return existingEntity;
            }
        }
        catch (Exception ex) { Debug.Write(ex.Message); }
        return null!;
    }

/// <summary>
/// Deletes an existing entity from database.
/// </summary>
/// <param name="expression">Uses an expression to fins the entity to delete.</param>
/// <returns>Returns true if delete was succesful, else false.</returns>
    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var existingEntity = await _context.Set<TEntity>().FirstOrDefaultAsync(expression);
            if (existingEntity != null)
            {
                _context.Set<TEntity>().Remove(existingEntity);
                await _context.SaveChangesAsync();
                return true;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;

    }

/// <summary>
/// Checks if a specific entity that matches the given statement, exists in the database
/// </summary>
/// <param name="expression">Uses an expression to find the entity</param>
/// <returns>Returns true if a matching entity exist, else false.</returns>
    public virtual async Task<bool> ExistingAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            var existing = await _context.Set<TEntity>().AnyAsync(expression);
            return existing;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }
}
