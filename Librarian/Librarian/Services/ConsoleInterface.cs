using System;
using System.Collections.Generic;
using System.Linq;
using Librarian.Models;
using Librarian.Services;

namespace Librarian.Services
{
    public class ConsoleInterface
    {
        private readonly LibraryService _libraryService;
        private bool _running;

        public ConsoleInterface()
        {
            _libraryService = new LibraryService();
            _running = true;
        }

        public void Run()
        {
            DisplayWelcomeMessage();

            while (_running)
            {
                DisplayMainMenu();
                ProcessMainMenuChoice();
            }

            _libraryService.Dispose();
        }

        private void DisplayWelcomeMessage()
        {
            Console.Clear();
            Console.WriteLine("=======================================");
            Console.WriteLine("      LIBRARY MANAGEMENT SYSTEM       ");
            Console.WriteLine("=======================================");
            Console.WriteLine();
        }

        private void DisplayMainMenu()
        {
            Console.WriteLine("\n--- MAIN MENU ---");
            Console.WriteLine("1. Add Resource");
            Console.WriteLine("2. Search Resources");
            Console.WriteLine("3. Borrow Resource");
            Console.WriteLine("4. Return Resource");
            Console.WriteLine("5. View Reports");
            Console.WriteLine("6. Manage Resources");
            Console.WriteLine("0. Exit");
            Console.Write("\nEnter your choice: ");
        }

        private void ProcessMainMenuChoice()
        {
            string input = Console.ReadLine()?.Trim();

            switch (input)
            {
                case "1":
                    AddResourceMenu();
                    break;
                case "2":
                    SearchResourcesMenu();
                    break;
                case "3":
                    BorrowResourceMenu();
                    break;
                case "4":
                    ReturnResourceMenu();
                    break;
                case "5":
                    ViewReportsMenu();
                    break;
                case "6":
                    ManageResourcesMenu();
                    break;
                case "0":
                    _running = false;
                    Console.WriteLine("thank you for using the library management system!");
                    break;
                default:
                    Console.WriteLine("invalid choice. please try again.");
                    break;
            }
        }

        private void AddResourceMenu()
        {
            Console.Clear();
            Console.WriteLine("--- ADD NEW RESOURCE ---");

            Console.Write("title: ");
            string title = Console.ReadLine()?.Trim();

            Console.Write("author/creator: ");
            string author = Console.ReadLine()?.Trim();

            Console.Write("publication year: ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("invalid year format.");
                PauseForUser();
                return;
            }

            Console.Write("genre/category: ");
            string genre = Console.ReadLine()?.Trim();

            Console.WriteLine("\nresource types:");
            Console.WriteLine("1. Book");
            Console.WriteLine("2. Journal");
            Console.WriteLine("3. Media");
            Console.Write("select type (1-3): ");

            ResourceType type;
            string typeInput = Console.ReadLine()?.Trim();
            switch (typeInput)
            {
                case "1":
                    type = ResourceType.Book;
                    break;
                case "2":
                    type = ResourceType.Journal;
                    break;
                case "3":
                    type = ResourceType.Media;
                    break;
                default:
                    Console.WriteLine("invalid resource type.");
                    PauseForUser();
                    return;
            }

            if (_libraryService.AddResource(title, author, year, genre, type))
            {
                Console.WriteLine("resource added successfully!");
            }
            else
            {
                Console.WriteLine("failed to add resource. please check your input.");
            }

            PauseForUser();
        }

        private void SearchResourcesMenu()
        {
            Console.Clear();
            Console.WriteLine("--- SEARCH RESOURCES ---");
            Console.WriteLine("1. Search by Title");
            Console.WriteLine("2. Search by Author");
            Console.WriteLine("3. Search by Genre");
            Console.WriteLine("4. Search by Year Range");
            Console.WriteLine("5. View All Resources");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("\nenter your choice: ");

            string choice = Console.ReadLine()?.Trim();
            List<LibraryResource> results = new List<LibraryResource>();

            switch (choice)
            {
                case "1":
                    Console.Write("enter title (partial search supported): ");
                    string title = Console.ReadLine()?.Trim();
                    results = _libraryService.SearchByTitlePartial(title);
                    break;

                case "2":
                    Console.Write("enter author name: ");
                    string author = Console.ReadLine()?.Trim();
                    results = _libraryService.SearchByAuthor(author);
                    break;

                case "3":
                    Console.Write("enter genre: ");
                    string genre = Console.ReadLine()?.Trim();
                    results = _libraryService.SearchByGenre(genre);
                    break;

                case "4":
                    Console.Write("enter minimum year: ");
                    if (int.TryParse(Console.ReadLine(), out int minYear))
                    {
                        Console.Write("enter maximum year: ");
                        if (int.TryParse(Console.ReadLine(), out int maxYear))
                        {
                            results = _libraryService.SearchByYearRange(minYear, maxYear);
                        }
                    }
                    break;

                case "5":
                    results = _libraryService.GetAllResourcesSorted();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("invalid choice.");
                    PauseForUser();
                    return;
            }

            DisplaySearchResults(results);
        }

        private void DisplaySearchResults(List<LibraryResource> resources)
        {
            Console.WriteLine($"\n--- SEARCH RESULTS ({resources.Count} found) ---");

            if (!resources.Any())
            {
                Console.WriteLine("no resources found matching your criteria.");
            }
            else
            {
                foreach (var resource in resources)
                {
                    Console.WriteLine(resource.ToString());
                }
            }

            PauseForUser();
        }

        private void BorrowResourceMenu()
        {
            Console.Clear();
            Console.WriteLine("--- BORROW RESOURCE ---");

            Console.Write("enter resource ID: ");
            if (!int.TryParse(Console.ReadLine(), out int resourceId))
            {
                Console.WriteLine("invalid resource ID.");
                PauseForUser();
                return;
            }

            var resource = _libraryService.GetResourceById(resourceId);
            if (resource == null)
            {
                Console.WriteLine("resource not found.");
                PauseForUser();
                return;
            }

            if (!resource.IsAvailable)
            {
                Console.WriteLine("resource is currently not available.");
                PauseForUser();
                return;
            }

            Console.WriteLine($"resource: {resource.Title} by {resource.Author}");
            Console.Write("borrower name: ");
            string borrowerName = Console.ReadLine()?.Trim();

            if (_libraryService.BorrowResource(resourceId, borrowerName))
            {
                Console.WriteLine("resource borrowed successfully!");
                Console.WriteLine("due date: " + DateTime.Now.AddDays(14).ToString("dd/MM/yyyy"));
            }
            else
            {
                Console.WriteLine("failed to borrow resource.");
            }

            PauseForUser();
        }

        private void ReturnResourceMenu()
        {
            Console.Clear();
            Console.WriteLine("--- RETURN RESOURCE ---");

            Console.Write("enter resource ID: ");
            if (!int.TryParse(Console.ReadLine(), out int resourceId))
            {
                Console.WriteLine("invalid resource ID.");
                PauseForUser();
                return;
            }

            var resource = _libraryService.GetResourceById(resourceId);
            if (resource == null)
            {
                Console.WriteLine("resource not found.");
                PauseForUser();
                return;
            }

            if (resource.IsAvailable)
            {
                Console.WriteLine("resource is not currently borrowed.");
                PauseForUser();
                return;
            }

            if (_libraryService.ReturnResource(resourceId))
            {
                Console.WriteLine("resource returned successfully!");
            }
            else
            {
                Console.WriteLine("failed to return resource.");
            }

            PauseForUser();
        }

        private void ViewReportsMenu()
        {
            Console.Clear();
            Console.WriteLine("--- REPORTS ---");
            Console.WriteLine("1. Overdue Items");
            Console.WriteLine("2. Currently Borrowed Items");
            Console.WriteLine("3. Resources by Category");
            Console.WriteLine("4. Library Statistics");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("\nenter your choice: ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    ShowOverdueReport();
                    break;
                case "2":
                    ShowBorrowedReport();
                    break;
                case "3":
                    ShowCategoryReport();
                    break;
                case "4":
                    ShowStatisticsReport();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("invalid choice.");
                    PauseForUser();
                    break;
            }
        }

        private void ShowOverdueReport()
        {
            Console.Clear();
            Console.WriteLine("--- OVERDUE ITEMS REPORT ---");

            var overdueItems = _libraryService.GetOverdueResources();

            if (!overdueItems.Any())
            {
                Console.WriteLine("no overdue items found.");
            }
            else
            {
                Console.WriteLine($"total overdue items: {overdueItems.Count}\n");
                foreach (var item in overdueItems)
                {
                    Console.WriteLine($"[{item.Resource.Id}] {item.Resource.Title}");
                    Console.WriteLine($"borrower: {item.BorrowerName}");
                    Console.WriteLine($"due date: {item.DueDate:dd/MM/yyyy}");
                    Console.WriteLine($"days overdue: {item.DaysOverdue()}");
                    Console.WriteLine();
                }
            }

            PauseForUser();
        }

        private void ShowBorrowedReport()
        {
            Console.Clear();
            Console.WriteLine("--- CURRENTLY BORROWED ITEMS ---");

            var borrowedItems = _libraryService.GetBorrowedResources();

            if (!borrowedItems.Any())
            {
                Console.WriteLine("no items currently borrowed.");
            }
            else
            {
                Console.WriteLine($"total borrowed items: {borrowedItems.Count}\n");
                foreach (var item in borrowedItems)
                {
                    Console.WriteLine($"[{item.Resource.Id}] {item.Resource.Title}");
                    Console.WriteLine($"borrower: {item.BorrowerName}");
                    Console.WriteLine($"borrowed: {item.BorrowDate:dd/MM/yyyy}");
                    Console.WriteLine($"due: {item.DueDate:dd/MM/yyyy}");
                    Console.WriteLine();
                }
            }

            PauseForUser();
        }

        private void ShowCategoryReport()
        {
            Console.Clear();
            Console.WriteLine("--- RESOURCES BY CATEGORY ---");

            var categoryStats = _libraryService.GetResourcesByCategory();

            if (!categoryStats.Any())
            {
                Console.WriteLine("no resources found.");
            }
            else
            {
                Console.WriteLine("category breakdown:\n");
                foreach (var category in categoryStats.OrderBy(c => c.Key))
                {
                    Console.WriteLine($"{category.Key}: {category.Value} resources");
                }
            }

            PauseForUser();
        }

        private void ShowStatisticsReport()
        {
            Console.Clear();
            Console.WriteLine("--- LIBRARY STATISTICS ---");

            int totalResources = _libraryService.GetTotalResourceCount();
            int availableResources = _libraryService.GetAvailableResourceCount();
            int borrowedResources = totalResources - availableResources;
            var overdueCount = _libraryService.GetOverdueResources().Count;

            Console.WriteLine($"total resources: {totalResources}");
            Console.WriteLine($"available resources: {availableResources}");
            Console.WriteLine($"borrowed resources: {borrowedResources}");
            Console.WriteLine($"overdue resources: {overdueCount}");

            if (totalResources > 0)
            {
                double utilization = (double)borrowedResources / totalResources * 100;
                Console.WriteLine($"utilization rate: {utilization:F1}%");
            }

            PauseForUser();
        }

        private void ManageResourcesMenu()
        {
            Console.Clear();
            Console.WriteLine("--- MANAGE RESOURCES ---");
            Console.WriteLine("1. Update Resource");
            Console.WriteLine("2. Remove Resource");
            Console.WriteLine("0. Back to Main Menu");
            Console.Write("\nenter your choice: ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    UpdateResourceMenu();
                    break;
                case "2":
                    RemoveResourceMenu();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("invalid choice.");
                    PauseForUser();
                    break;
            }
        }

        private void UpdateResourceMenu()
        {
            Console.Clear();
            Console.WriteLine("--- UPDATE RESOURCE ---");

            Console.Write("enter resource ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int resourceId))
            {
                Console.WriteLine("invalid resource ID.");
                PauseForUser();
                return;
            }

            var resource = _libraryService.GetResourceById(resourceId);
            if (resource == null)
            {
                Console.WriteLine("resource not found.");
                PauseForUser();
                return;
            }

            Console.WriteLine($"current details: {resource}");
            Console.WriteLine();

            Console.Write($"new title (current: {resource.Title}): ");
            string title = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(title))
                title = resource.Title;

            Console.Write($"new author (current: {resource.Author}): ");
            string author = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(author))
                author = resource.Author;

            Console.Write($"new year (current: {resource.PublicationYear}): ");
            string yearInput = Console.ReadLine()?.Trim();
            int year = resource.PublicationYear;
            if (!string.IsNullOrEmpty(yearInput) && !int.TryParse(yearInput, out year))
            {
                Console.WriteLine("invalid year format.");
                PauseForUser();
                return;
            }

            Console.Write($"new genre (current: {resource.Genre}): ");
            string genre = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(genre))
                genre = resource.Genre;

            if (_libraryService.UpdateResource(resourceId, title, author, year, genre))
            {
                Console.WriteLine("resource updated successfully!");
            }
            else
            {
                Console.WriteLine("failed to update resource.");
            }

            PauseForUser();
        }

        private void RemoveResourceMenu()
        {
            Console.Clear();
            Console.WriteLine("--- REMOVE RESOURCE ---");

            Console.Write("enter resource ID to remove: ");
            if (!int.TryParse(Console.ReadLine(), out int resourceId))
            {
                Console.WriteLine("invalid resource ID.");
                PauseForUser();
                return;
            }

            var resource = _libraryService.GetResourceById(resourceId);
            if (resource == null)
            {
                Console.WriteLine("resource not found.");
                PauseForUser();
                return;
            }

            Console.WriteLine($"resource to remove: {resource}");
            Console.Write("are you sure? (y/n): ");
            string confirmation = Console.ReadLine()?.Trim().ToLower();

            if (confirmation == "y" || confirmation == "yes")
            {
                if (_libraryService.RemoveResource(resourceId))
                {
                    Console.WriteLine("resource removed successfully!");
                }
                else
                {
                    Console.WriteLine("failed to remove resource (may be currently borrowed).");
                }
            }
            else
            {
                Console.WriteLine("removal cancelled.");
            }

            PauseForUser();
        }

        private void PauseForUser()
        {
            Console.WriteLine("\npress any key to continue...");
            Console.ReadKey();
        }
    }
}
