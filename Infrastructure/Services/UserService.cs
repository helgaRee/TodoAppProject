using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Services;

public class UserService(UserRepository userRepository)
{

    private readonly UserRepository _userRepository = userRepository;

    /// <summary>
    /// Creates a new user if there is no user with a specific email already.
    /// </summary>
    /// <param name="userRegistrationDto"></param>
    /// <returns>Returns a UserDto if created, else null.</returns>
    public async Task<UserDto> CreateUserAsync(UserRegistrationDto userRegistrationDto)
    {
        try
        {
            if (!await _userRepository.ExistingAsync(x => x.Email == userRegistrationDto.Email))
            {
                var userEntity = new UserEntity
                {
                    UserName = userRegistrationDto.UserName,
                    Email = userRegistrationDto.Email,
                    Password = userRegistrationDto.Password,
                };
                var userDto = new UserDto(
                    Id: userEntity.Id, 
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

    /// <summary>
    /// Gets a specific user.
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>Returns a UserDto, else null.</returns>
    public async Task<UserDto> GetUserAsync(Expression<Func<UserEntity, bool>> expression)
    {
        try
        {
            var userEntity = await _userRepository.GetAsync(expression);
            if (userEntity != null)
            {
                var userDto = new UserDto(
                    Id: userEntity.Id,
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

    /// <summary>
    /// Gets all the users from database and converts each to a UserDto.
    /// </summary>
    /// <returns>A list of UserDto's, else null.</returns>
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


    /// <summary>
    /// Updates an existing user in database.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Returns an updated UserDto, else null.</returns>
    public async Task<UserDto> UpdateUser(UpdateUserDto dto)
    {
        try
        {
            var userEntity = new UserEntity { Id = dto.Id, UserName = dto.UserName, Email = dto.Email };

             var updatedUserEntity = await _userRepository.UpdateAsync(x => x.Id == dto.Id, userEntity);
            if (updatedUserEntity != null)
            {
                var userDto = new UserDto(dto.Id, dto.UserName, dto.Email);
                return userDto;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    /// <summary>
    /// Deletes a UserEntity
    /// </summary>
    /// <param name="expression"></param>
    /// <returns>Returns true if the entity was deleted, else null.</returns>
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
