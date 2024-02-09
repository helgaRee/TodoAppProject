using Infrastructure.Dtos;
using Infrastructure.Entities;
using Infrastructure.Services;
using System.Diagnostics.Eventing.Reader;

namespace Presentation.ConsoleApp;

public class ConsoleUI
{
    private readonly TaskService _taskService;

    public ConsoleUI(TaskService taskService)
    {
        _taskService = taskService;
    }


    //Metod - visar användarmenyn
    public async Task ShowMenuAsync()
    {

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"{"",-50}## Todo App ##");
            Console.WriteLine("");
            Console.WriteLine("Where do you wanna start?");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("");
            Console.WriteLine($"{"1.",-3} Registrera ny todo");
            Console.WriteLine($"{"2.",-3} Visa dina todos");
            Console.WriteLine($"{"3.",-3} Ändra en todo");
            Console.WriteLine($"{"4.",-3} Ta bort en todo");
            Console.WriteLine($"{"5.",-3} Avsluta programmet");
            Console.WriteLine("");
            Console.WriteLine("-------------------------------");


            var userChoice = Console.ReadLine();

            switch (userChoice)
            {
                case "1":
                    await CreateTask_UIAsync();
                    break;
                case "2":
                    await GetTasks_UIAsync();
                    break;
                case "3":
                    await UpdateTask_UIAsync();
                    break;
                case "4":
                    await DeleteTask_UIAsync();
                    break;
                case "5":
                    await ExitProgramAsync();
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, försök igen.");
                    break;
            }
            Console.ReadKey();
        }
    }


    public async Task CreateTask_UIAsync()
    {
        var dto = new TaskUpdateDto();


        Console.Clear();
        await DisplayTitleAsync("Skapa ny todo");
        Console.WriteLine("--Skriv först in dina inloggninsuppgifter--");

        Console.Write("Username: ");
        dto.UserName = Console.ReadLine()!;

        Console.Write("Email: ");
        dto.Email = Console.ReadLine()!;

        Console.Write("Password: ");
        dto.Password = Console.ReadLine()!;

        Console.WriteLine("--Skapa din todo--");
        Console.WriteLine("");

        Console.Write("Todo titel: ");
        dto.Title = Console.ReadLine()!;

        Console.Write("Under kategori: ");
        dto.CategoryName = Console.ReadLine()!;

        Console.Write("Beskrivning: ");
        dto.Description = Console.ReadLine()!;

        Console.Write("Deadline: ");
        dto.Deadline = Console.ReadLine()!;

        Console.WriteLine("Prioritet (1-3): ");
        dto.PriorityLevel = Console.ReadLine()!;

        Console.Write("Plats för todo: ");
        dto.LocationName = Console.ReadLine()!;

        Console.Write("Adress: ");
        dto.StreetName = Console.ReadLine()!;

        Console.Write("Postkod: ");
        dto.PostalCode = Console.ReadLine()!;

        Console.Write("Stad: ");
        dto.City = Console.ReadLine()!;

        Console.Write("Status: ");
        dto.Status = Console.ReadLine()!;


        var result = await _taskService.CreateTaskAsync(dto);

        if (result)
        {
            Console.Clear();
            Console.WriteLine($"{dto.Title} har skapats.");
            Console.ReadKey();
        }
    }

    public async Task GetTasks_UIAsync()
    {
        Console.Clear();
        await DisplayTitleAsync("Registrerade todos");
        var tasks = await _taskService.GetTasksAsync();
            Console.Clear();
        await DisplayTitleAsync("Registrerade todos");


        if (tasks.Any())
        {
            await DisplayTitleAsync("Att göra");
            foreach (var task in tasks)
            {
                Console.WriteLine($" - Titel: {task.Title}\n   Beskrivning: {task.Description}\n   Deadline: {task.Deadline}\n");
            }
            Console.WriteLine("");
            Console.WriteLine("---------------------------------------");
            Console.WriteLine($" * Ange 'Delete' för att ta bort\n * För att se detaljer om en todo, ange 'Details'\n * Tryck enter för att återgå till menyn.");
            string userChoice = Console.ReadLine()!;

            switch (userChoice.ToLower())
            {
                case "delete":
                    await DeleteTask_UIAsync();
                    Console.ReadKey();
                    break;
                case "details":
                    await GetDetailsTaskAsync();
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine($"Du skickas tillbaka till menyn.");
                    break;
            }
        }
        else
        {
            Console.WriteLine($"Det finns inga uppgifter i listan.\nTryck enter för att återgå till huvudmenyn.");
        }
        Console.ReadKey();
    }

    public async Task GetDetailsTaskAsync()
    {
        await DisplayTitleAsync("Granska en uppgift");

        var tasks = await _taskService.GetTasksAsync();
        Console.Clear();

        Console.WriteLine("Här är en lista över tillgängliga uppgifter:");
        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Title}");
        }

        Console.WriteLine("Ange titeln på den uppgift du vill granska");
        string selectedTitle = Console.ReadLine()?.Trim()!;

        var selectedTask = tasks.FirstOrDefault(task => task.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase));

        if (selectedTask != null)
        {
            Console.Clear();

            await DisplayTitleAsync($"Kategori {selectedTask.CategoryName}");
            Console.WriteLine($"Uppgift titel: {selectedTask.Title}");
            Console.WriteLine($"Beskrivning: {selectedTask.Description}");
            Console.WriteLine($"Deadline: {selectedTask.Deadline}");
            Console.WriteLine($"Status: {selectedTask.Status}");
            Console.WriteLine($"Plats: {selectedTask.LocationName}");
            Console.WriteLine($"Prioritet: {selectedTask.PriorityLevel}");
            Console.WriteLine($"Gatuadress: {selectedTask.StreetName}");
            Console.WriteLine($"Postnummer: {selectedTask.PostalCode}");
            Console.WriteLine($"Stad: {selectedTask.City}");
            Console.WriteLine($"Användarnamn: {selectedTask.UserName}");
            Console.WriteLine($"E-post: {selectedTask.Email}");
            Console.WriteLine($"Lösenord: {selectedTask.Password}");

            Console.WriteLine("");
            Console.WriteLine("Tryck enter för att återgå till menyn.");
            Console.ReadKey();
        }
        else
        {
            Console.WriteLine($"Det finns ingen uppgift med titeln \"{selectedTitle}\".");
            Console.WriteLine("Tryck enter för att återgå till menyn.");
            Console.ReadKey();
        }
    }



    public async Task UpdateTask_UIAsync()
    {
        Console.Clear();
        await DisplayTitleAsync("Uppdatera en uppgift");
       
        while (true)
        {
            var tasks = await _taskService.GetTasksAsync();
            Console.Clear();
            await DisplayTitleAsync("Uppdatera en uppgift");
            foreach (var task in tasks)
            {
                Console.WriteLine($"{"",-2}* {task.Title}");
                Console.WriteLine("");
            }

            Console.WriteLine("Ange titeln på den uppgift du vill uppdatera");
            string selectedTitle = Console.ReadLine()?.Trim()!;

            var selectedTask = tasks.FirstOrDefault(task => task.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase));

            if (selectedTask != null)
            {
                Console.Clear();
                Console.WriteLine("----------------------------------------");

                Console.WriteLine("Befintlig information");
                Console.WriteLine("----------------------------------------");

                Console.WriteLine($"Uppgift titel: {selectedTask.Title}");
                Console.WriteLine($"Beskrivning: {selectedTask.Description}");
                Console.WriteLine($"Deadline: {selectedTask.Deadline}");
                Console.WriteLine($"Status: {selectedTask.Status}");
                Console.WriteLine($"Plats: {selectedTask.LocationName}");
                Console.WriteLine($"Kategori: {selectedTask.CategoryName}");
                Console.WriteLine($"IsPrivate? {selectedTask.IsPrivate}");
                Console.WriteLine($"Prioritet: {selectedTask.PriorityLevel}");
                Console.WriteLine($"Adress: {selectedTask.StreetName}");
                Console.WriteLine($"Postkod: {selectedTask.PostalCode}");
                Console.WriteLine($"Stad: {selectedTask.City}");
 
                Console.WriteLine("");
                Console.WriteLine("----------------------------------------");

                Console.WriteLine("Vilken information vill du uppdatera?");
                Console.WriteLine("----------------------------------------");

                Console.WriteLine("1. Titel");
                Console.WriteLine("2. Beskrivning");
                Console.WriteLine("3. Deadline");
                Console.WriteLine("4. Status");
                Console.WriteLine("5. Plats");
                Console.WriteLine("6. Kategori");
                Console.WriteLine("7. Privat");
                Console.WriteLine("8. Prioritet");
                Console.WriteLine("9. Address");
                Console.WriteLine("10. Postkod");
                Console.WriteLine("11. Stad");

                Console.WriteLine("12. Avbryt");

                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        Console.Write("Ny titel: ");
                        string newTitle = Console.ReadLine()!;
                        selectedTask.Title = newTitle;
                        break;
                    case "2":
                        Console.Write("Ny beskrivning: ");
                        string newDescription = Console.ReadLine()!;
                        selectedTask.Description = newDescription;
                        break;
                    case "3":
                        Console.Write("Ny deadline: ");
                        string newDeadline = Console.ReadLine()!;
                        selectedTask.Deadline = newDeadline;
                        break;
                    case "4":
                        Console.Write("Ny status: ");
                        string newStatus = Console.ReadLine()!;
                        selectedTask.Status = newStatus;
                        break;
                    case "5":
                        Console.Write("Ny plats: ");
                        string newLocation = Console.ReadLine()!;
                        selectedTask.Status = newLocation;
                        break;
                    case "6":
                        Console.Write("Ny kategori: ");
                        string newCategory = Console.ReadLine()!;
                        selectedTask.Status = newCategory;
                        break;
                    case "7":
                        Console.Write("Är uppgiften privat? (ja/nej): ");
                        string input = Console.ReadLine()?.ToLower()!; // Läs inmatningen och konvertera den till små bokstäver

                        bool isPrivate; // Variabel för att lagra om uppgiften är privat eller inte

                        // Jämför inmatningen och sätt isPrivate till true om det är "ja", annars false
                        if (input == "ja")
                        {
                            isPrivate = true;
                        }
                        else if (input == "nej")
                        {
                            isPrivate = false;
                        }
                        else
                        {
                            Console.WriteLine("Ogiltig inmatning. Ange 'ja' eller 'nej'.");
                            break; // Avsluta switch-satsen om inmatningen är ogiltig
                        }

                        selectedTask.IsPrivate = isPrivate; // Tilldela värdet av isPrivate till selectedTask.IsPrivate
                        break;
                    case "8":
                        Console.Write("Ny prio: ");
                        string newPriority = Console.ReadLine()!;
                        selectedTask.PriorityLevel = newPriority;
                        break;
                    case "9":
                        Console.Write("Ny Address: ");
                        string newAddress = Console.ReadLine()!;
                        selectedTask.StreetName = newAddress;
                        break;
                    case "10":
                        Console.Write("Ny Postkod: ");
                        string newPostalCode = Console.ReadLine()!;
                        selectedTask.PostalCode = newPostalCode;
                        break;
                    case "11":
                        Console.Write("Ny Stad: ");
                        string newCity = Console.ReadLine()!;
                        selectedTask.City = newCity;
                        break;
                    case "12":
                        Console.WriteLine("Uppdatering avbruten.");
                        return;
                    default:
                        Console.WriteLine("Ogiltigt val.");
                        break;
                }
            

            var updatedTask = new TaskUpdateDto
            {
                Id = selectedTask.Id,
                Title = selectedTask.Title,
                Description = selectedTask.Description!,
                Deadline = selectedTask.Deadline!,
                Status = selectedTask.Status!,
                CategoryName = selectedTask.CategoryName,
                IsPrivate = selectedTask.IsPrivate,
                PriorityLevel = selectedTask.PriorityLevel,
                LocationName = selectedTask.LocationName,
                StreetName = selectedTask.StreetName,
                City = selectedTask.City,
                PostalCode = selectedTask.PostalCode,
            };
            var updated = await _taskService.UpdateTaskAsync(updatedTask);

            if (updated != null)
            {
                Console.Clear();
                Console.WriteLine("Uppgiften har uppdaterats.");
            }
            else
            {
                Console.WriteLine("Något gick fel. Uppgiften kunde inte uppdateras.");
            }
        }
        else
        {
            Console.WriteLine($"Det finns ingen uppgift med titeln \"{selectedTitle}\".");
        }
        }
    }


    public async Task DeleteTask_UIAsync()
    {
        Console.Clear();

        await DisplayTitleAsync("Ta bort en uppgift");

        var tasks = await _taskService.GetTasksAsync();

        if (tasks.Any())
        {
            Console.WriteLine("Befintliga uppgifter");
            Console.WriteLine("\n");

            foreach (var task in tasks)
            {
                Console.WriteLine($"{task.CategoryName} {task.Title} {task.Status}");
            }

            Console.WriteLine("\n");
            Console.WriteLine("Ange titel för den uppgift du vill ta bort, eller enter för att gå tillbaka till menyn.");
            string userInput = Console.ReadLine()!;

            Console.WriteLine("Ångrat dig? Ange 'avbryt'");
            string cancelOption = Console.ReadLine()?.ToLower()!;

            if (cancelOption == "avbryt")
            {
                await ReturnToMenuAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(userInput))
                {
                    var taskToDelete = tasks.FirstOrDefault(task => task.Title.Equals(userInput, StringComparison.OrdinalIgnoreCase));

                    if (taskToDelete != null)
                    {
                        Console.Clear();
                        bool deleteSuccess = await _taskService.DeleteTaskAsync(taskToDelete.Id);


                        if (deleteSuccess)
                        {
                            Console.WriteLine($"Uppgiften med titeln '{taskToDelete.Title}' har tagits bort.");
                        }
                        else
                        {
                            Console.WriteLine($"Något gick fel. Uppgiften med titeln '{taskToDelete.Title}' hittades inte eller kunde inte tas bort.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Ingen uppgift med titeln '{userInput}' hittades.");
                    }

                }
                else
                {
                    Console.WriteLine($"Felaktig inmatning, du skickas tillbaka till huvudmenyn..");
                }
            }
        }
        else
        {
            Console.WriteLine($"Det finns inga uppgifter inlaggda i appen.\nTryck enter för att återgå till huvudmenyn.");
        }
    }

    public async Task ExitProgramAsync()
    {
        Console.Clear();
        await DisplayTitleAsync("Stäng av programmet");

        Console.WriteLine($"Är du säker på att du vill avsluta? (ja/nej)");
        string userOption = Console.ReadLine()?.ToLower()!;

        if (userOption != "ja")
        {
            Console.WriteLine($"Tryck enter för att gå tillbaka till huvudmenyn.");
        }
        if (userOption != "ja" && userOption != "nej")
        {
            Console.Clear();
            Console.WriteLine($"Ops, nu blev det fel.. du skickas tillbaka till menyn.");
        }
        else
        {
            Environment.Exit(0);
        }

    }

    /// <summary>
    /// Display title for each method
    /// </summary>
    /// <param name="title">Specific title, type of a string</param>
    /// <returns>A specific title for each method</returns>
    public async Task DisplayTitleAsync(string title)
    {
        Console.Clear();
        Console.WriteLine($"{"",-50}## Todo App ##");
        Console.WriteLine("");
        Console.WriteLine($"## {title} ##");
        Console.WriteLine("---------------------------------------");
        Console.WriteLine("");
    }


    /// <summary>
    /// Return to main menu
    /// </summary>
    /// <param name=""></param>
    /// <returns>Returning to main menu</returns>
    public async Task ReturnToMenuAsync()
    {
        Console.WriteLine("");
        Console.WriteLine($"Ange 'meny' för att gå tillbaka.");
        string userChoice = Console.ReadLine() ?? "";

        if (userChoice.ToLower() == "meny")
        {
            // Console.ReadKey();
            Console.WriteLine($"Du skickas tillbaka till huvudmenyn.");
        }
    }

}
