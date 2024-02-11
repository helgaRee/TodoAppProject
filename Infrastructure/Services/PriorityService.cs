using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class PriorityService(PriorityRepository priorityRepository)
{
    private readonly PriorityRepository _priorityRepository = priorityRepository;

    /// <summary>
    /// Creates a new priorityEntity
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns True if a new entity was created, else null.</returns>
    public async Task<bool> CreatePriorityAsync(int id)
    {
        try
        {
            var priorityExists = await _priorityRepository.ExistingAsync(x => x.Id == id);
            if (!priorityExists)
            {
                var priorityEntity = await _priorityRepository.CreateAsync(new PriorityEntity { Id = id });
                if (priorityEntity != null)
                {
                    return true;
                }
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return false;
    }

    /// <summary>
    /// Gets a specific priority.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>Returns a PriorityDto with information about the priority, else null.</returns>
    public async Task<PriorityDto> GetPriorityAsync(Expression<Func<PriorityEntity, bool>> predicate)
    {
        try
        {
            var priorityEntity = await _priorityRepository.GetAsync(predicate);
            if (priorityEntity != null)
            {
                var priorityDto = new PriorityDto(priorityEntity.Id, priorityEntity.PriorityLevel);
                return priorityDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

    /// <summary>
    /// Gets all the priorityEntities from the database and converts them to PriorityDtos.
    /// </summary>
    /// <returns>Returns a list of PriorityDtos, else null.</returns>
    public async Task<IEnumerable<PriorityDto>> GetPrioritiesAsync()
    {
        try
        {
            var priorityEntities = await _priorityRepository.GetAllAsync();

            if (priorityEntities != null)
            {
                var list = new List<PriorityDto>();
                foreach (var priorityEntity in priorityEntities)
                    list.Add(new PriorityDto(priorityEntity.Id, priorityEntity.PriorityLevel));
                return list;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

    /// <summary>
    /// Updates an existing priorityEntity
    /// </summary>
    /// <param name="updatedPriority"></param>
    /// <returns>Returns an updated PriorityDto, else null.</returns>
    public async Task<PriorityDto> UpdatePriorityAsync(PriorityDto updatedPriority)
    {
        try
        {
            var priorityEntity = new PriorityEntity { Id = updatedPriority.Id, PriorityLevel = updatedPriority.PriorityLevel};

            var updatedPriorityEntity = await _priorityRepository.UpdateAsync(x => x.Id == updatedPriority.Id, priorityEntity);
            if (updatedPriorityEntity != null)
            {
                var priorityDto = new PriorityDto(updatedPriority.Id, updatedPriority.PriorityLevel);
                return priorityDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

/// <summary>
/// Deletes a priorityEntity.
/// </summary>
/// <param name="expression"></param>
/// <returns>Returns true if deleted, else false.</returns>
    public async Task<bool> DeletePriorityAsync(Expression<Func<PriorityEntity, bool>> expression)
    {
        try
        {
            var result = await _priorityRepository.DeleteAsync(expression);
            return result;
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return false;
    }
}


