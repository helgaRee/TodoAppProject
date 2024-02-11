using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class LocationService(LocationRepository locationRepository)
{
    private readonly LocationRepository _locationRepository = locationRepository;

/// <summary>
/// Creates a new location if it doesnt exist in the database.
/// </summary>
/// <param name="entity">Takes in a parameter of locationentity and creates a new from it.</param>
/// <returns>Returns true if a new location was created succesfully, else false.</returns>
    public async Task<bool> CreateLocationAsync(LocationEntity entity)
    {
        try
        {
            var locationEntity = await _locationRepository.GetAsync(x => x.Id == entity.Id);
            if (locationEntity == null)
            {
                var newLocation = await _locationRepository.CreateAsync(new LocationEntity
                {
                    Id = entity.Id,
                    LocationName = entity.LocationName,
                    StreetName = entity.StreetName,
                    PostalCode = entity.PostalCode,
                    City = entity.City,
                });
                return true;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }

/// <summary>
/// Gets a specific locationEntity and convert it to a LocationDto
/// </summary>
/// <param name="expression">Uses an expression to search</param>
/// <returns>Returns a locationDto if found, else null.</returns>
    public async Task<LocationDto> GetLocationAsync(Expression<Func<LocationEntity, bool>> expression)
    {
        try
        {
            var locationEntity = await _locationRepository.GetAsync(expression);
            if (locationEntity != null)
            {
                var locationDto = new LocationDto(locationEntity.Id, locationEntity.LocationName!, locationEntity.StreetName!, locationEntity.PostalCode!, locationEntity.City!);
                return locationDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

/// <summary>
/// Gets all locations from the database. Converts every LocationEntity to a LocationDto.
/// </summary>
/// <returns>Returns the LocationDto's in a list, else null.</returns>
    public async Task<IEnumerable<LocationDto>> GetLocationsAsync()
    {
        try
        {
            var locationEntities = await _locationRepository.GetAllAsync();
            if (locationEntities != null)
            {
                var list = new List<LocationDto>();

                foreach(var locationEntity in locationEntities)
                {
                    list.Add(new LocationDto(locationEntity.Id, locationEntity.LocationName!, locationEntity.StreetName!, locationEntity.PostalCode!, locationEntity.City!));
                    return list;
                }
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    /// <summary>
    /// Updates an existing location in database. 
    /// </summary>
    /// <param name="updatedLocation"></param>
    /// <returns>Returns the updated LocationDto, else null.</returns>
    public async Task<LocationDto> UpdateLocationAsync(LocationDto updatedLocation)
    {
        try
        {
            var locationEntity = new LocationEntity
            {
                Id = updatedLocation.LocationId,
                LocationName = updatedLocation.LocationName,
                StreetName = updatedLocation.StreetName,
                PostalCode = updatedLocation.PostalCode,
                City = updatedLocation.City
            };         
            var updatedLocationEntity = await _locationRepository.UpdateAsync(x => x.Id == updatedLocation.LocationId, locationEntity);
            if (updatedLocationEntity != null)
            {
                var locationDto = new LocationDto
                    (
                       updatedLocation.LocationId,
                       updatedLocation.LocationName,
                       updatedLocation.StreetName,
                       updatedLocation.PostalCode,
                       updatedLocation.City
                    );
                return locationDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    /// <summary>
    /// Deletes a LocationEntity
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>True if delete was succesfull, else false.</returns>
    public async Task<bool> DeleteLocationAsync(Expression<Func<LocationEntity, bool>> expression)
    {
        try
        {
            var result = await _locationRepository.DeleteAsync(expression);
            return result;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }
}
