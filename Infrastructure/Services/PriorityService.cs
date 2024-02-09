using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class PriorityService(PriorityRepository priorityRepository)
{
    private readonly PriorityRepository _priorityRepository = priorityRepository;


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

    public async Task<PriorityDto> GetPriorityAsync(Expression<Func<PriorityEntity, bool>> predicate)
    {
        try
        {
            //försöker Hämta entitet
            var priorityEntity = await _priorityRepository.GetAsync(predicate);
            if (priorityEntity != null)
            {
                //om hämtning lyckas, omvandla entiteten till en Dto med Id och Cname
                var priorityDto = new PriorityDto(priorityEntity.Id, priorityEntity.PriorityLevel);
                return priorityDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }

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

    public async Task<PriorityDto> UpdatePriorityAsync(PriorityDto updatedPriority)
    {
        try
        {
            //försöker Hämta entitet baserat på Id (den gör en hämtning via BaseRepo) och uppdaterar
            var priorityEntity = new PriorityEntity { Id = updatedPriority.Id, PriorityLevel = updatedPriority.PriorityLevel};

            var updatedPriorityEntity = await _priorityRepository.UpdateAsync(x => x.Id == updatedPriority.Id, priorityEntity);
            //om uppdateringen lyckades
            if (updatedPriorityEntity != null)
            {
                //skapa ny CategoryDto
                var priorityDto = new PriorityDto(updatedPriority.Id, updatedPriority.PriorityLevel);
                return priorityDto;
            }

        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + (ex.Message)); }
        return null!;
    }


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


