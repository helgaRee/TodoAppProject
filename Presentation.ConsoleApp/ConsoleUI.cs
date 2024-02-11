using Infrastructure.Repositories;
using Infrastructure.Services;


namespace Presentation.ConsoleApp;

public class ConsoleUI(TaskService taskService, CategoryService categoryService, TaskRepository taskRepository, UserService userService)
{
    private readonly TaskService _taskService = taskService;
    private readonly CategoryService _categoryService = categoryService;
    private readonly TaskRepository _taskRepository = taskRepository;
    private readonly UserService _userService = userService;

    /// <summary>
    /// A menu to let the user choose how to navigate through the app
    /// </summary>
    /// <returns>A menu with options for user</returns>
    public async Task ShowMenuAsync()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"{"",-50}## Todo App ##");
            Console.WriteLine("");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Vad vill du göra?");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("");
            Console.WriteLine($"{"1.",-3} Registrera ny todo");
            Console.WriteLine($"{"2.",-3} Visa dina todos");
            Console.WriteLine($"{"3.",-3} Ändra en todo");
            Console.WriteLine($"{"4.",-3} Ta bort en todo");
            Console.WriteLine($"{"5.",-3} Visa kategorier");
            Console.WriteLine($"{"6.",-3} Visa alla användare");
            Console.WriteLine($"{"7.",-3} Avsluta programmet");
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
                    await GetCategories_UIAsync();
                    break;
                case "6":
                    await GetUsers_UIAsync();
                    break;
                case "7":
                    await ExitProgramAsync();
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, försök igen.");
                    break;
            }
            Console.ReadKey();
        }
    }

    /// <summary>
    /// A method to create a new task.
    /// </summary>
    /// <returns> True with a message if a new task was created, else false. </returns>
    public async Task CreateTask_UIAsync()
    {
        var dto = new TaskUpdateDto();

        Console.Clear();
        DisplayTitleAsync("Skapa ny todo");
        Console.WriteLine(" |-------------------------------------------------|");
        Console.WriteLine("  Skriv först in dina inloggninsuppgifter");
        Console.WriteLine(" |-------------------------------------------------|");
        Console.Write("Username: ");
        var username = dto.UserName = Console.ReadLine()!;

        Console.Write("Email: ");
        dto.Email = Console.ReadLine()!;

        Console.Write("Password: ");
        dto.Password = Console.ReadLine()!;
        Console.WriteLine("");
        Console.WriteLine(" ---------------------------------------------------------------------------------");
        Console.WriteLine($" Hej {username}! Nu kan du skapa en ny uppgift. Fyll i valfri information.");
        Console.WriteLine(" ---------------------------------------------------------------------------------");
        Console.WriteLine("");

        Console.Write("Titel: ");
        dto.Title = Console.ReadLine()!;

        Console.Write("kategori: ");
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
            Console.WriteLine("------------------------------");
            Console.WriteLine($"{dto.Title} har skapats.");
            Console.WriteLine("------------------------------");
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Lets the user get all the registerered users.
    /// </summary>
    /// <returns>A list of the existing users.</returns>
    public async Task GetUsers_UIAsync()
    {
        var users = await _userService.GetUsersAsync();
        Console.Clear();
        DisplayTitleAsync("Användare");

        if (users.Any())
        {
            foreach (var user in users)
            {
                Console.WriteLine(user.UserName);
            }
            Console.WriteLine("");
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine($" * Ange 'Details' för att se en användares sparade uppgifter\n * Tryck enter för att återgå till menyn.");
            Console.WriteLine("--------------------------------------------------------------");

            string userChoice = Console.ReadLine()!;

            switch (userChoice.ToLower())
            {
                case "details":
                    Console.WriteLine("Ange den användare du vill öppna: ");
                    string userName = Console.ReadLine()!;
                    await GetTasksFromUserAsync(userName);
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine($"Du skickas tillbaka till menyn.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Det finns inga användare i listan.\nTryck enter för att återgå till huvudmenyn.");
            Console.WriteLine("-------------------------------------");
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Gets all the tasks a specific user has registered.
    /// </summary>
    /// <param name="userName">specifies the user</param>
    /// <returns>A list of the tasks a specific user has registered.</returns>
    public async Task GetTasksFromUserAsync(string userName)
    {       
        var tasksInUser = await _taskRepository.GetTasksForUserAsync(userName);
        Console.Clear();
        DisplayTitleAsync($"'{userName}'s sparade uppgifter");

        if (tasksInUser != null)
        {
            foreach (var task in tasksInUser)
            {
                Console.WriteLine($"Title: {task.Title}");

                if (!string.IsNullOrEmpty(task.Description))
                {
                    Console.WriteLine($"Description: {task.Description}");
                }
                if (!string.IsNullOrEmpty(task.Deadline))
                {
                    Console.WriteLine($"Deadline: {task.Deadline}");
                }
                if (!string.IsNullOrEmpty(task.Status))
                {
                    Console.WriteLine($"Status: {task.Status}");
                }
                Console.WriteLine("---------------------------------------");
            }
        }
        else
        {
            Console.WriteLine("-------------------------------------------------------------");
            Console.WriteLine($"Det finns inga sparade uppgifter för användaren '{userName}'.\nTryck enter för att återgå till huvudmenyn.");
            Console.WriteLine("--------------------------------------------------------------");
        }
        Console.WriteLine("Tryck enter för att återgå till huvudmenyn.");
        Console.ReadKey();
    }

    /// <summary>
    /// Gets all categories
    /// </summary>
    /// <returns>A list of the existing categories</returns>
    public async Task GetCategories_UIAsync()
    {
        var categories = await _categoryService.GetCategoriesAsync();
        Console.Clear();
        DisplayTitleAsync("kategorier");

        if (categories.Any())
        {
            foreach (var category in categories)
            {
                Console.WriteLine(category.CategoryName);
            }
            Console.WriteLine("");
            Console.WriteLine("----------------------------------");
            Console.WriteLine($" * Ange 'Delete' för att ta bort\n * Ange 'Details' för att öppna en kategori\n * Tryck enter för att återgå till menyn.");
            Console.WriteLine("----------------------------------");

            string userChoice = Console.ReadLine()!;

            switch (userChoice.ToLower())
            {
                case "delete":
                    await DeleteCategory_UIAsync();
                    Console.ReadKey();
                    break;
                case "details":
                    Console.WriteLine("Ange namnet på kategorin för att visa uppgifter: ");
                    string categoryName = Console.ReadLine()!;
                    await GetTasksFromCategoryAsync(categoryName);
                    Console.ReadKey();
                    break;
                default:
                    Console.WriteLine($"Du skickas tillbaka till menyn.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Det finns inga uppgifter i listan.\nTryck enter för att återgå till huvudmenyn.");
            Console.WriteLine("------------------------------------");
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Gets all the tasks from a certain category.
    /// </summary>
    /// <param name="categoryName">Specifies the category the user wants to open.</param>
    /// <returns>A list of tasks connected to a specific category chosen by user.</returns>
    public async Task GetTasksFromCategoryAsync(string categoryName)
    {
        var tasksInCategory = await _taskRepository.GetTasksForCategoryAsync(categoryName);
        Console.Clear();
        DisplayTitleAsync($"Uppgifter i kategorin '{categoryName}'");

        if (tasksInCategory != null)
        {
            foreach (var task in tasksInCategory)
            {
                Console.WriteLine($"Title: {task.Title}");

                if (!string.IsNullOrEmpty(task.Description))
                {
                    Console.WriteLine($"Description: {task.Description}");
                }
                if(!string.IsNullOrEmpty(task.Deadline))
                {
                    Console.WriteLine($"Deadline: {task.Deadline}");
                }
                if(!string.IsNullOrEmpty(task.Status))
                {
                    Console.WriteLine($"Status: {task.Status}");
                }
                Console.WriteLine("---------------------------------------");
            }
        }
        else
        {
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine($"Det finns inga uppgifter under kategorin '{categoryName}'.\nTryck enter för att återgå till huvudmenyn.");
            Console.WriteLine("-----------------------------------------------------------");
        }
        Console.WriteLine("Tryck enter för att återgå till huvudmenyn.");
        Console.ReadKey();
    }


    /// <summary>
    /// Gets all tasks added by all users.
    /// </summary>
    /// <returns>A list of all tasks.</returns>
    public async Task GetTasks_UIAsync()
    {
        var tasks = await _taskService.GetTasksAsync();
        Console.Clear();
        DisplayTitleAsync("Registrerade todos");

        if (tasks.Any())
        {
            foreach (var task in tasks)
            {
                Console.WriteLine($" -- {task.CategoryName.ToUpper()} --\n   Titel: {task.Title}\n   Beskrivning: {task.Description}\n   Deadline: {task.Deadline}\n");
            }
            Console.WriteLine("");

            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($" * Ange 'Delete' för att ta bort\n * För att se detaljer om en todo, ange 'Details'\n * Tryck enter för att återgå till menyn.");
            Console.WriteLine("----------------------------------------------------");

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
                    Console.WriteLine("--------------------------------");
                    Console.WriteLine($"Du skickas tillbaka till menyn.");
                    break;
            }
        }
        else
        {
            Console.WriteLine("------------------------------------");
            Console.WriteLine($"Det finns inga uppgifter i listan.\nTryck enter för att återgå till huvudmenyn.");
            Console.WriteLine("------------------------------------");
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Gives the user all information about a certain task.
    /// </summary>
    /// <returns>A list of all the details from a specific task chosen by user.</returns>
    public async Task GetDetailsTaskAsync()
    {
        var tasks = await _taskService.GetTasksAsync();
        Console.Clear();
        Console.WriteLine("----------------------------------------");

        DisplayTitleAsync("Lista över tillgängliga uppgifter:");

        foreach (var task in tasks)
        {
            Console.WriteLine($" * {task.Title}");
        }
        Console.WriteLine("");

        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Ange titeln på den uppgift du vill granska");
        Console.WriteLine("------------------------------------------");
        string selectedTitle = Console.ReadLine()?.Trim()!;
        Console.WriteLine("------------------------------------------");

        var selectedTask = tasks.FirstOrDefault(task => task.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase));

        if (selectedTask != null)
        {
            Console.Clear();

            DisplayTitleAsync($"{selectedTask.CategoryName}");
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
            Console.WriteLine("");

            Console.WriteLine("--------------------------------------");
            Console.WriteLine("Tryck Enter för att återgå till menyn.");
            Console.WriteLine("--------------------------------------");

            Console.ReadKey();
        }
        else
        {
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine($"Det finns ingen uppgift med titeln \"{selectedTitle}\".");
            Console.WriteLine("Tryck enter för att återgå till menyn.");
            Console.WriteLine("--------------------------------------------------------");
            Console.ReadKey();
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Lets the user update information about a certain task.
    /// </summary>
    /// <returns>True is update was succesfull, else false with a message.</returns>
    public async Task UpdateTask_UIAsync()
    {
        while (true)
        {
            var tasks = await _taskService.GetTasksAsync();
            Console.Clear();
            DisplayTitleAsync("Uppdatera en uppgift");
            foreach (var task in tasks)
            {
                Console.WriteLine($"{"",-2}* {task.Title}");
                Console.WriteLine("");
            }
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine("Ange titeln på den uppgift du vill uppdatera");
            Console.WriteLine("--------------------------------------------");

            string selectedTitle = Console.ReadLine()?.Trim()!;

            var selectedTask = tasks.FirstOrDefault(task => task.Title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase));

            if (selectedTask != null)
            {
                Console.Clear();
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Befintlig information");
                Console.WriteLine("----------------------------------------");

                Console.WriteLine("");
                Console.WriteLine($"Titel: {selectedTask.Title}");
                Console.WriteLine($"Beskrivning: {selectedTask.Description}");
                Console.WriteLine($"Deadline: {selectedTask.Deadline}");
                Console.WriteLine($"Status: {selectedTask.Status}");
                Console.WriteLine($"Plats: {selectedTask.LocationName}");
                Console.WriteLine($"Kategori: {selectedTask.CategoryName}");
                Console.WriteLine($"Privat? {selectedTask.IsPrivate}");
                Console.WriteLine($"Prioritet: {selectedTask.PriorityLevel}");
                Console.WriteLine($"Adress: {selectedTask.StreetName}");
                Console.WriteLine($"Postkod: {selectedTask.PostalCode}");
                Console.WriteLine($"Stad: {selectedTask.City}");
                Console.WriteLine("");

                Console.WriteLine("----------------------------------------");
                Console.WriteLine("Vilken information vill du uppdatera? (1-10)");
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
                        selectedTask.LocationName = newLocation;
                        break;
                    case "6":
                        Console.Write("Ny kategori: ");
                        string newCategory = Console.ReadLine()!;
                        selectedTask.CategoryName = newCategory;
                        break;
                    case "7":
                        Console.Write("Är uppgiften privat? (ja/nej): ");
                        string input = Console.ReadLine()?.ToLower()!; 

                        bool isPrivate; 

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
                            break;
                        }
                        selectedTask.IsPrivate = isPrivate; 

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
                    Console.WriteLine("----------------------------------------");
                    Console.WriteLine("Uppgiften har uppdaterats.");
                    Console.WriteLine("----------------------------------------");
                }
                else
                {
                    Console.WriteLine("-----------------------------------------------");
                    Console.WriteLine("Något gick fel. Uppgiften kunde inte uppdateras.");
                    Console.WriteLine("-----------------------------------------------");
                }
            }
            else
            {
                Console.WriteLine("--------------------------------------------------------");
                Console.WriteLine($"Det finns ingen uppgift med titeln \"{selectedTitle}\".");
                Console.WriteLine("--------------------------------------------------------");
            }
        Console.WriteLine("Vill du fortsätta uppdatera fler uppgifter? (ja/nej): ");
        string continueInput = Console.ReadLine()?.ToLower()!;

        if (continueInput == "nej")
        {
            break;
        }
        else if (continueInput != "ja")
        {
            Console.WriteLine("Ogiltig inmatning. Ange 'ja' eller 'nej'.");
        }
            Console.ReadKey();
        }
    }

    /// <summary>
    /// Deletes a category by Categoryname
    /// </summary>
    /// <returns>True if deletion was succesfull, else false.</returns>
    public async Task DeleteCategory_UIAsync()
    {
        Console.Clear();
        var categories = await _categoryService.GetCategoriesAsync();
        DisplayTitleAsync("Ta bort en kategori");
        
        if (categories.Any())
        {
            Console.WriteLine("Befintliga Kategorier");
            Console.WriteLine("\n");

            foreach (var category in categories)
            {
                Console.WriteLine($"* {category.CategoryName}\n");
            }
            Console.WriteLine("\n");
            Console.WriteLine("---------------------------------------------------------------------------------");
            Console.WriteLine("Ange vilken kategori du vill ta bort, eller enter för att gå tillbaka till menyn.");
            Console.WriteLine("---------------------------------------------------------------------------------");
            string userInput = Console.ReadLine()!;
            Console.WriteLine("--------------------------------------------------------------");
            Console.WriteLine("Ångrat dig? Ange 'avbryt', annars tryck enter för att bekräfta");
            Console.WriteLine("--------------------------------------------------------------");
            string cancelOption = Console.ReadLine()?.ToLower()!;

            if (cancelOption == "avbryt")
            {
                ReturnToMenuAsync();
            }
            else
            {
                if (!string.IsNullOrEmpty(userInput))
                {
                    var categoryToDelete = categories.FirstOrDefault(category => category.CategoryName.Equals(userInput, StringComparison.OrdinalIgnoreCase));

                    if (categoryToDelete != null)
                    {
                        Console.Clear();
                        bool deleteSuccess = await _categoryService.DeleteCategoryAsync(categoryToDelete.Id);

                        Console.Clear();

                        if (deleteSuccess)
                        {
                            Console.WriteLine("-------------------------------------------------------------");
                            Console.WriteLine($"Kategorin '{categoryToDelete.CategoryName}' har tagits bort.");
                            Console.WriteLine("-------------------------------------------------------------");
                        }
                        else
                        {
                            Console.WriteLine("--------------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine($"Något gick fel. Kategorin '{categoryToDelete.CategoryName}' hittades inte eller kunde inte tas bort. \nFörsök igen.");
                            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                            Console.ReadKey();
                            await DeleteCategory_UIAsync();
                        }
                    }
                    else
                    {
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine($"Ingen kategori med namnet '{userInput}' hittades. \nFörsök igen.");
                        Console.WriteLine("--------------------------------------------------");
                        Console.ReadKey();
                        await DeleteCategory_UIAsync();

                    }
                }
            }
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    /// <returns>Returns true if succesfull, else false.</returns>
        public async Task DeleteTask_UIAsync()
        {
            var tasks = await _taskService.GetTasksAsync();
            Console.Clear();
            DisplayTitleAsync("Ta bort en uppgift");

            if (tasks.Any())
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("Befintliga uppgifter");
                Console.WriteLine("--------------------");
                Console.WriteLine("\n");

                foreach (var task in tasks)
                {
                    if (task!= null)
                    {
                        if (task.CategoryName != null)
                        {
                            Console.WriteLine( "--"+task.CategoryName.ToUpper() +"--");
                        }
                        if (task.Title != null)
                        {
                            Console.WriteLine(task.Title + " ");
                        }
                        if (task.Status != null)
                        {
                            Console.WriteLine(task.Status + " " + "\n");
                    }
                }             
            }
            Console.WriteLine("\n");
            Console.WriteLine("---------------------------------------------------------------------------------------");
            Console.WriteLine("Ange titel för den uppgift du vill ta bort, eller 'meny' för att gå tillbaka till menyn.\n ");
            Console.WriteLine("---------------------------------------------------------------------------------------");
            string userInput = Console.ReadLine()!;

                if (userInput == "meny")
                {
                    ReturnToMenuAsync();
                }
                else
                {
                    if (!string.IsNullOrEmpty(userInput))
                    {
                        var taskToDelete = tasks.FirstOrDefault(task => task.Title.Equals(userInput, StringComparison.OrdinalIgnoreCase));

                        if (taskToDelete != null)
                        {
                            bool deleteSuccess = await _taskService.DeleteTaskAsync(taskToDelete.Id);
                            Console.Clear();

                            if (deleteSuccess)
                            {
                                Console.WriteLine("-------------------------------------------------------------");
                                Console.WriteLine($"Uppgiften med titeln '{taskToDelete.Title}' har tagits bort.\n Gå vidare med enter.");
                                Console.WriteLine("-------------------------------------------------------------");
                            Console.ReadKey();
                        }
                            else
                            {
                                Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                                Console.WriteLine($"Något gick fel. Uppgiften med titeln '{taskToDelete.Title}' hittades inte eller kunde inte tas bort.");
                                Console.WriteLine("-----------------------------------------------------------------------------------------------------");
                        }
                        }
                        else
                        {
                            Console.WriteLine("-------------------------------------------------");
                            Console.WriteLine($"Ingen uppgift med titeln '{userInput}' hittades.");
                            Console.WriteLine("-------------------------------------------------");
                    }
                    }
                    else
                    {
                        Console.WriteLine("----------------------------------------------------------");
                        Console.WriteLine($"Felaktig inmatning, du skickas tillbaka till huvudmenyn..");
                        Console.WriteLine("----------------------------------------------------------");
                }
                }
            }
            else
            {
                Console.WriteLine("----------------------------------------------------------------------------------------");
                Console.WriteLine($"Det finns inga uppgifter inlaggda i appen.\nTryck enter för att återgå till huvudmenyn.");
                Console.WriteLine("----------------------------------------------------------------------------------------");
        }
        Console.ReadKey();
    }

    /// <summary>
    /// Method to exit program
    /// </summary>
    /// <returns>True if exit succeded, else false.</returns>
        public async Task ExitProgramAsync()
        {
            Console.Clear();
            DisplayTitleAsync("Stäng av programmet");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"Är du säker på att du vill avsluta? (ja/nej)");
            Console.WriteLine("---------------------------------------------");
            string userOption = Console.ReadLine()?.ToLower()!;

            if (userOption != "ja")
            {
                Console.WriteLine("-------------------------------------------------");
                Console.WriteLine($"Tryck enter för att gå tillbaka till huvudmenyn.");
                Console.WriteLine("-------------------------------------------------");
        }
            if (userOption != "ja" && userOption != "nej")
            {
                Console.Clear();
                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine($"Ops, nu blev det fel.. du skickas tillbaka till menyn.");
                Console.WriteLine("-------------------------------------------------------");
        }
            else
            {
                Environment.Exit(0);
            }
        await Task.Delay(0);
    }

        /// <summary>
        /// Display title for each method
        /// </summary>
        /// <param name="title">Specific title, type of a string</param>
        /// <returns>A specific title for each method</returns>
        public void DisplayTitleAsync(string title)
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
        public void ReturnToMenuAsync()
        {
            Console.WriteLine("");
            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Ange 'meny' för att gå tillbaka.");
            Console.WriteLine("---------------------------------");
        string userChoice = Console.ReadLine() ?? "";

            if (userChoice.ToLower() == "meny")
            {
                Console.WriteLine("-------------------------------------");
                Console.WriteLine($"Du skickas tillbaka till huvudmenyn.");
                Console.WriteLine("-------------------------------------");
                Console.ReadKey();
            }
        }
    }

