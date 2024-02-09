using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
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


    //CREATE
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


    //READ ONE
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

    //READ ALL
    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        try
        {
            //Hämta entiteterna från databasen
            var existingEntities = await _context.Set<TEntity>().ToListAsync();
            await _context.SaveChangesAsync();
            return existingEntities;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    //UPDATE
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


    //DELETE
    public virtual async Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            //söker
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


    //Existing entity
    public virtual async Task<bool> ExistingAsync(Expression<Func<TEntity, bool>> expression)
    {
        try
        {
            //finns entiteten?
            var existing = await _context.Set<TEntity>().AnyAsync(expression);
            return existing;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;

    }

}
