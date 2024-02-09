using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class UserService(UserRepository userRepository)
{
    //Läs ut repo
    private readonly UserRepository _userRepository = userRepository;

    //vill returnera en Dto som inte har skapats än!
    public async Task<UserDto> CreateUserAsync(UserRegistrationDto userRegistrationDto)
    {
        try
        {
            //kolla först om det redan finns en user, isåfall hämta den! annars skapa ny
            if (!await _userRepository.ExistingAsync(x => x.Email == userRegistrationDto.Email))
            {
                //finns inte usern, skapa en ny. ID skapas automatiskt
                var userEntity = new UserEntity
                {
                    UserName = userRegistrationDto.UserName,
                    Email = userRegistrationDto.Email,
                    Password = userRegistrationDto.Password,
                };

                // Skapa en UserDto och fyll den med användaruppgifterna
                var userDto = new UserDto(
                    Id: userEntity.Id, // Namngiven parameterlista
                    UserName: userEntity.UserName,
                    Email: userEntity.Email
                );

                return userDto;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        return null!;
    }

    public async Task<UserDto> GetUserAsync(Expression<Func<UserEntity, bool>> expression)
    {
        try
        {
            var userEntity = await _userRepository.GetAsync(expression);
            if (userEntity != null)
            {
                var userDto = new UserDto(
                    Id: userEntity.Id, // Namngiven parameterlista
                    UserName: userEntity.UserName,
                    Email: userEntity.Email
                );
                return userDto;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        return null!;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        try
        {
            var userEntities = await _userRepository.GetAllAsync();

            if (userEntities != null)
            {
                var list = new List<UserDto>();
                foreach(var userEntity in userEntities)            
                    list.Add(new UserDto(userEntity.Id, userEntity.UserName, userEntity.Email));

                    return list;
                
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }


    public async Task<UserDto> UpdateUser(UpdateUserDto dto)
    {
        try
        {

            //skapa en ny user för uppdatering
            var userEntity = new UserEntity { Id = dto.Id, UserName = dto.UserName, Email = dto.Email };

             var updatedUserEntity = await _userRepository.UpdateAsync(x => x.Id == dto.Id, userEntity);
            if (updatedUserEntity != null)
            {
                //Uppdatering sker nu
                var userDto = new UserDto(dto.Id, dto.UserName, dto.Email);
                return userDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    public async Task<bool> DeleteUserAsync(Expression<Func<UserEntity, bool>> expression)
    {
        try
        {
            var result = await _userRepository.DeleteAsync(expression);
            return result;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }

}
