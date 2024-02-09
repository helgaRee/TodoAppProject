using Infrastructure.Contexts;
using Infrastructure.Dtos;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.ConsoleApp;

var builder = Host.CreateDefaultBuilder().ConfigureServices(services =>
{
    services.AddDbContext<DataContext>(x => x.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\EC\SQL\TodoAppProject\Infrastructure\Data\local_database.mdf;Integrated Security=True;Connect Timeout=30"));

    services.AddScoped<CategoryRepository>();
    services.AddScoped<CategoryService>();
    services.AddScoped<TaskRepository>();
    services.AddScoped<TaskService>();
    services.AddScoped<UserRepository>();
    services.AddScoped<UserService>();
    services.AddScoped<PriorityRepository>();
    services.AddScoped<PriorityService>();
    services.AddScoped<LocationRepository>();
    services.AddScoped<LocationService>();

    services.AddSingleton<ConsoleUI>();
}).Build();

var consoleUI = builder.Services.GetRequiredService<ConsoleUI>();
await consoleUI.ShowMenuAsync();
await consoleUI.CreateTask_UIAsync();
await consoleUI.GetTasks_UIAsync();
await consoleUI.GetDetailsTaskAsync();
await consoleUI.ExitProgramAsync();
// Skapa en instans av TaskUpdateDto och ge den värden
var taskToUpdate = new TaskUpdateDto
{
    Id = 1, // Tilldela den aktuella uppgiftens ID
    Title = "Ny titel",
    Description = "Ny beskrivning",
    Deadline = "Ny deadline",
    Status = "Ny status",
    CategoryName = "Ny kategori",
    // Fyll i resten av egenskaperna enligt behov
};

await consoleUI.UpdateTask_UIAsync();