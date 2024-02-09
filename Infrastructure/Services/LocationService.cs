using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Infrastructure.Services;

public class LocationService(LocationRepository locationRepository)
{
    private readonly LocationRepository _locationRepository = locationRepository;


    //CREATE
    public async Task<bool> CreateLocationAsync(LocationEntity entity)
    {
        try
        {
            //skapa location om den inte redan finns
            var locationEntity = await _locationRepository.GetAsync(x => x.Id == entity.Id);
            if (locationEntity == null)
            {
                //skapa ny location och mappa in egenskaper
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


    //GET ONE
    public async Task<LocationDto> GetLocationAsync(Expression<Func<LocationEntity, bool>> expression)
    {
        try
        {
            var locationEntity = await _locationRepository.GetAsync(expression);
            if (locationEntity != null)
            {
                //om hämtning lyckas, omvandla entiteten till en Dto med Id och Cname
                var locationDto = new LocationDto(locationEntity.Id, locationEntity.LocationName!, locationEntity.StreetName!, locationEntity.PostalCode!, locationEntity.City!);
                return locationDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }



    //GETALL
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



    //UPDATE
    public async Task<LocationDto> UpdateLocationAsync(LocationDto updatedLocation)
    {
        try
        {
            //försöker Hämta entitet baserat på Id (den gör en hämtning via BaseRepo) och uppdaterar
            var locationEntity = new LocationEntity
            {
                Id = updatedLocation.LocationId,
                LocationName = updatedLocation.LocationName,
                StreetName = updatedLocation.StreetName,
                PostalCode = updatedLocation.PostalCode,
                City = updatedLocation.City
            };
            
            //spara in de uppdaterade uppgifterna till den nya Locationen, uppdatera!
            var updatedLocationEntity = await _locationRepository.UpdateAsync(x => x.Id == updatedLocation.LocationId, locationEntity);
            //mappa in
            if (updatedLocationEntity != null)
            {
                //skapa ny CategoryDto
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
