using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Repositories;
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

    /// <summary>
    /// Creates a new task if it doesnt already exist.
    /// </summary>
    /// <param name="taskDto"></param>
    /// <returns>Returns true if a task was created, else false.</returns>
        public async Task<bool> CreateTaskAsync(TaskUpdateDto taskDto)
    {
        try
        {
            var taskInDatabase = await _taskRepository.GetAsync(x => x.Title == taskDto.Title);

            if (taskInDatabase == null)
            {
                // Location
                var locationEntity = await _locationRepository.GetAsync(x => x.LocationName == taskDto.LocationName);
                locationEntity ??= await _locationRepository.CreateAsync(new LocationEntity
                {
                    LocationName = taskDto.LocationName,
                    StreetName = taskDto.StreetName,
                    PostalCode = taskDto.PostalCode,
                    City = taskDto.City
                });

                // Priority
                var priorityEntity = await _priorityRepository.GetAsync(x => x.PriorityLevel == taskDto.PriorityLevel);
                priorityEntity ??= await _priorityRepository.CreateAsync(new PriorityEntity
                {
                    PriorityLevel = taskDto.PriorityLevel,
                });

                // User
                var userEntity = await _userRepository.GetAsync(x => x.UserName == taskDto.UserName);
                userEntity ??= await _userRepository.CreateAsync(new UserEntity
                {
                    UserName = taskDto.UserName,
                    Email = taskDto.Email!,
                    Password = taskDto.Password!
                });

                // Category
                var categoryEntity = await _categoryRepository.GetAsync(x => x.CategoryName == taskDto.CategoryName);
                categoryEntity ??= await _categoryRepository.CreateAsync(new CategoryEntity
                {
                    CategoryName = taskDto.CategoryName,
                    IsPrivate = taskDto.IsPrivate
                });

                if (locationEntity == null || priorityEntity == null || userEntity == null || categoryEntity == null)
                {
                    Debug.WriteLine("En eller flera entiteter kunde inte skapas.");
                    return false;
                }

                // TaskEntity
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
    /// Gets an existing task from the database
    /// </summary>
    /// <param name="title"></param>
    /// <returns>Returns an existing taskDto from the database, or null if the database is empty</returns>
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
    /// Gets all tasks from database.
    /// </summary>
    /// <param name=""></param>
    /// <returns>Returns a list of existing tasks or an empty list.</returns>
    public async Task<IEnumerable<TaskDto>> GetTasksAsync()
    {
        try
        {
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
                    CategoryName = taskEntity.Category.CategoryName,
                    IsPrivate = taskEntity.Category.IsPrivate.GetValueOrDefault(),
                    PriorityLevel = taskEntity.Priority!.PriorityLevel, 
                    LocationName = taskEntity.Location!.LocationName, 
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
    /// Updates an existing task
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Returns a new, updated TaskUpdateDto, else null.</returns>
    public async Task<TaskUpdateDto> UpdateTaskAsync(TaskUpdateDto updatedTask)
    {
        try
        {
            var existingTask = await _taskRepository.GetAsync(x => x.Id == updatedTask.Id);

            if (existingTask != null)
            {
                existingTask.Title = updatedTask.Title;
                existingTask.Description = updatedTask.Description;
                existingTask.Deadline = updatedTask.Deadline;
                existingTask.Status = updatedTask.Status;

                var result = await _taskRepository.UpdateAsync(x => x.Id == existingTask.Id, existingTask);

                if (result != null)
                {
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
}


