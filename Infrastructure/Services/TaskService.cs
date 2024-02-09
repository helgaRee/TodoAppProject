using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Diagnostics;

namespace Infrastructure.Services;

public class TaskService
{
    private readonly LocationRepository _locationRepository;
    private readonly PriorityRepository _priorityRepository;
    private readonly UserRepository _userRepository;
    private readonly CategoryRepository _categoryRepository;
    private readonly TaskRepository _taskRepository;

    public TaskService(LocationRepository locationRepository, PriorityRepository priorityRepository,
        UserRepository userRepository, CategoryRepository categoryRepository, TaskRepository taskRepository)
    {
        _locationRepository = locationRepository;
        _priorityRepository = priorityRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _taskRepository = taskRepository;
    }


        public async Task<bool> CreateTaskAsync(TaskUpdateDto taskDto)
    {
        try
        {
            // Kontrollera om  redan existerar i databasen - SÖKNING och HÄMTAR
            var taskInDatabase = await _taskRepository.GetAsync(x => x.Title == taskDto.Title);

            if (taskInDatabase == null)
            {

                // Kolla om Location redan finns, finns den inte, skapa, finns den, hämta
                var locationEntity = await _locationRepository.GetAsync(x => x.LocationName == taskDto.LocationName);
                locationEntity ??= await _locationRepository.CreateAsync(new LocationEntity
                {
                    LocationName = taskDto.LocationName,
                    StreetName = taskDto.StreetName,
                    PostalCode = taskDto.PostalCode,
                    City = taskDto.City
                });

                //skapa priority
                var priorityEntity = await _priorityRepository.GetAsync(x => x.PriorityLevel == taskDto.PriorityLevel);
                priorityEntity ??= await _priorityRepository.CreateAsync(new PriorityEntity
                {
                    PriorityLevel = taskDto.PriorityLevel,
                });

                // Skapa User om den inte redan finns, isåfall hämta
                var userEntity = await _userRepository.GetAsync(x => x.UserName == taskDto.UserName);
                userEntity ??= await _userRepository.CreateAsync(new UserEntity
                {
                    UserName = taskDto.UserName,
                    Email = taskDto.Email!,
                    Password = taskDto.Password!
                });

                // Skapa Category om den inte redan finns, isåfall hämta
                var categoryEntity = await _categoryRepository.GetAsync(x => x.CategoryName == taskDto.CategoryName);
                categoryEntity ??= await _categoryRepository.CreateAsync(new CategoryEntity
                {
                    CategoryName = taskDto.CategoryName,
                    IsPrivate = taskDto.IsPrivate
                });

                // Kontrollera att alla nödvändiga entiteter skapades korrekt innan du skapar taskEntity
                if (locationEntity == null || priorityEntity == null || userEntity == null || categoryEntity == null)
                {
                    Debug.WriteLine("En eller flera entiteter kunde inte skapas.");
                    return false;
                }

                // Skapa taskEntity och associera med Location, Calendar, User och Category
                var taskEntity = new TaskEntity
                {
                    Title = taskDto.Title,
                    Description = taskDto.Description,
                    Deadline = taskDto.Deadline,
                    Status = taskDto.Status,
                    LocationId = locationEntity.Id,
                    PriorityId = priorityEntity.Id,
                    UserId = userEntity.Id,
                    CategoryId = categoryEntity.Id
                };
                // Spara uppgiften i databasen
                var createdTaskEntity = await _taskRepository.CreateAsync(taskEntity);
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return false;
    }

    /// <summary>
    /// Gets a task from the database
    /// </summary>
    /// <param name="title"></param>
    /// <returns>An existing task from the database, or null if the database is empty</returns>
    public async Task<TaskDto> GetTaskAsync(string title)
    {
        try
        {
            var taskEntity = await _taskRepository.GetAsync(x => x.Title == title);
           if(taskEntity != null)
            {
                var taskDto = new TaskDto()
                {
                    Id = taskEntity.Id,
                    Title = taskEntity.Title,
                    Description = taskEntity.Description!,
                    Deadline = taskEntity.Deadline,
                    Status = taskEntity.Status!,
                    CategoryName = taskEntity.Category.CategoryName,
                    IsPrivate = taskEntity.Category.IsPrivate.GetValueOrDefault(),
                    LocationName = taskEntity.Location!.LocationName,
                    StreetName = taskEntity.Location.StreetName,
                    City = taskEntity.Location.City,
                    PostalCode = taskEntity.Location.PostalCode,
                    UserName = taskEntity.User.UserName,
                    Email = taskEntity.User.Email,
                    Password = taskEntity.User.Password,

                };
            return taskDto;
            }
            return null!;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return null!;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name=""></param>
    /// <returns>Returns a list of existing tasks or an empty list.</returns>
    public async Task<IEnumerable<TaskDto>> GetTasksAsync()
    {
        
        try
        {
            //Hämtar en lista av Taskentity-objekt från db
            var taskEntities = await _taskRepository.GetAllAsync();
            var taskDtos = new List<TaskDto>();

            foreach (var taskEntity in taskEntities)
            {
                var taskDto = new TaskDto
                {
                    Id = taskEntity.Id,
                    Title = taskEntity.Title,
                    Description = taskEntity.Description!,
                    Deadline = taskEntity.Deadline,
                    Status = taskEntity.Status!,
                    CategoryName = taskEntity.Category.CategoryName, // Exempel: Anta att kategorin är en separat entitet som har en egenskap 'Name'
                    IsPrivate = taskEntity.Category.IsPrivate.GetValueOrDefault(),
                    PriorityLevel = taskEntity.Priority!.PriorityLevel, // Antag att prioritet är en separat entitet med en egenskap 'Level'
                    LocationName = taskEntity.Location!.LocationName, // Antag att platsen är en separat entitet med en egenskap 'Name'
                    StreetName = taskEntity.Location?.StreetName,
                    City = taskEntity.Location?.City,
                    PostalCode = taskEntity.Location?.PostalCode,
                    UserName = taskEntity.User.UserName,
                    Email = taskEntity.User.Email,
                    Password = taskEntity.User.Password
                };

                taskDtos.Add(taskDto);
            }

            return taskDtos;
        }
        catch (Exception ex) { Debug.WriteLine(ex.Message); }
        return Enumerable.Empty<TaskDto>();
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    public async Task<TaskUpdateDto> UpdateTaskAsync(TaskUpdateDto updatedTask)
    {
        try
        {
            // Hämta den befintliga uppgiften från databasen baserat på Id
            var existingTask = await _taskRepository.GetAsync(x => x.Id == updatedTask.Id);

            if (existingTask != null)
            {
                // Uppdatera värdena från TaskUpdateDto till den befintliga uppgiften
                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                existingTask.Deadline = updatedTask.Deadline;
                existingTask.Status = updatedTask.Status;


                // Anropa UpdateAsync-metoden för att spara ändringarna i databasen
                var result = await _taskRepository.UpdateAsync(x => x.Id == existingTask.Id, existingTask);

                if (result != null)
                {
                    // Returnera en ny instans av TaskUpdateDto med de uppdaterade värdena
                    return new TaskUpdateDto
                    {
                        Id = existingTask.Id,
                        Title = existingTask.Title,
                        Description = existingTask.Description,
                        Deadline = existingTask.Deadline,
                        Status = existingTask.Status,
   
                    };
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
        }

        return null!;
    }


    /// <summary>
    /// Deletes an existing task by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Returns true if deletion was succesful, else false.</returns>
    public async Task<bool> DeleteTaskAsync(int id)
    {
        //hämta
        var deletedTask = await _taskRepository.DeleteAsync(x => x.Id == id);
        return true;

    }

    public async Task<bool> IsCompleteAsync(int id)
    {
        //hämta
        var completedTask = await _taskRepository.GetAsync(x => x.Id == id);
        if (completedTask != null)
        {
            //ska kunna bocka av denna att den nu är färdig.
        }
        return true;

    }

}


