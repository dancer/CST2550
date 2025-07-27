using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Librarian.Models;
using Librarian.Services;

namespace Librarian.Services
{
    public class ModernConsoleInterface
    {
        private readonly LibraryService _libraryService;
        private bool _running;
        private int _selectedIndex = 0;

        // color scheme
        private const ConsoleColor PRIMARY_COLOR = ConsoleColor.Cyan;
        private const ConsoleColor ACCENT_COLOR = ConsoleColor.Yellow;
        private const ConsoleColor SUCCESS_COLOR = ConsoleColor.Green;
        private const ConsoleColor ERROR_COLOR = ConsoleColor.Red;
        private const ConsoleColor TEXT_COLOR = ConsoleColor.White;
        private const ConsoleColor BACKGROUND_COLOR = ConsoleColor.Black;
        private const ConsoleColor SELECTED_COLOR = ConsoleColor.DarkCyan;

        public ModernConsoleInterface()
        {
            _libraryService = new LibraryService();
            _running = true;

            // setup console
            Console.CursorVisible = false;
            Console.BackgroundColor = BACKGROUND_COLOR;
            Console.ForegroundColor = TEXT_COLOR;
            Console.Clear();
        }

        public void Run()
        {
            ShowLoadingAnimation();
            DisplayWelcomeScreen();

            while (_running)
            {
                ShowMainMenu();
            }

            ShowExitAnimation();
            _libraryService.Dispose();
        }

        private void ShowLoadingAnimation()
        {
            Console.Clear();
            string[] frames = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };

            Console.SetCursorPosition(0, Console.WindowHeight / 2);
            CenterText("Loading Library Management System...", PRIMARY_COLOR);

            for (int i = 0; i < 20; i++)
            {
                Console.SetCursorPosition(
                    Console.WindowWidth / 2 - 1,
                    Console.WindowHeight / 2 + 2
                );
                Console.ForegroundColor = ACCENT_COLOR;
                Console.Write(frames[i % frames.Length]);
                Thread.Sleep(100);
            }

            Console.Clear();
        }

        private void DisplayWelcomeScreen()
        {
            Console.Clear();
            Console.ForegroundColor = PRIMARY_COLOR;

            string[] logo =
            {
                "╔══════════════════════════════════════════════════════════════╗",
                "║                                                              ║",
                "║    ██╗     ██╗██████╗ ██████╗  █████╗ ██████╗ ██╗ █████╗    ║",
                "║    ██║     ██║██╔══██╗██╔══██╗██╔══██╗██╔══██╗██║██╔══██╗   ║",
                "║    ██║     ██║██████╔╝██████╔╝███████║██████╔╝██║███████║   ║",
                "║    ██║     ██║██╔══██╗██╔══██╗██╔══██║██╔══██╗██║██╔══██║   ║",
                "║    ███████╗██║██████╔╝██║  ██║██║  ██║██║  ██║██║██║  ██║   ║",
                "║    ╚══════╝╚═╝╚═════╝ ╚═╝  ╚═╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝╚═╝  ╚═╝   ║",
                "║                                                              ║",
                "║              M A N A G E M E N T   S Y S T E M               ║",
                "║                                                              ║",
                "╚══════════════════════════════════════════════════════════════╝",
            };

            int startY = (Console.WindowHeight - logo.Length) / 2 - 5;

            for (int i = 0; i < logo.Length; i++)
            {
                Console.SetCursorPosition((Console.WindowWidth - logo[i].Length) / 2, startY + i);
                Console.WriteLine(logo[i]);
                Thread.Sleep(50);
            }

            Console.SetCursorPosition(0, startY + logo.Length + 2);
            CenterText("Modern Library Management Experience", ACCENT_COLOR);
            CenterText("CST2550 Coursework Project", ConsoleColor.Gray);

            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            CenterText("Press any key to continue...", ConsoleColor.DarkGray);

            Console.ReadKey(true);
        }

        private void ShowMainMenu()
        {
            string[] menuItems =
            {
                "📚 Add New Resource",
                "🔍 Search Library",
                "📖 Borrow Resource",
                "📕 Return Resource",
                "📊 View Reports",
                "⚙️  Manage Resources",
                "❌ Exit Application",
            };

            while (true)
            {
                Console.Clear();
                DrawHeader("MAIN MENU");
                DrawMenuBox(menuItems);

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        _selectedIndex = (_selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        _selectedIndex = (_selectedIndex + 1) % menuItems.Length;
                        break;

                    case ConsoleKey.Enter:
                        ExecuteMenuAction(_selectedIndex);
                        if (!_running)
                            return;
                        break;

                    case ConsoleKey.Escape:
                        _running = false;
                        return;
                }
            }
        }

        private void ExecuteMenuAction(int index)
        {
            switch (index)
            {
                case 0:
                    AddResourceMenu();
                    break;
                case 1:
                    SearchResourcesMenu();
                    break;
                case 2:
                    BorrowResourceMenu();
                    break;
                case 3:
                    ReturnResourceMenu();
                    break;
                case 4:
                    ViewReportsMenu();
                    break;
                case 5:
                    ManageResourcesMenu();
                    break;
                case 6:
                    _running = false;
                    break;
            }
        }

        private void AddResourceMenu()
        {
            Console.Clear();
            DrawHeader("ADD NEW RESOURCE");

            var fields = new Dictionary<string, string>
            {
                ["Title"] = "",
                ["Author/Creator"] = "",
                ["Publication Year"] = "",
                ["Genre/Category"] = "",
            };

            // interactive form
            foreach (var field in fields.Keys.ToList())
            {
                fields[field] = GetUserInput(
                    $"Enter {field}:",
                    field == "Publication Year" ? "number" : "text"
                );
                if (fields[field] == null)
                    return; // user pressed escape
            }

            // resource type selection
            string[] types = { "📚 Book", "📰 Journal", "💿 Media" };
            int typeIndex = ShowSelectionMenu("Select Resource Type:", types);
            if (typeIndex == -1)
                return;

            ResourceType resourceType = (ResourceType)typeIndex;

            // validation
            if (!int.TryParse(fields["Publication Year"], out int year))
            {
                ShowMessage("Invalid year format!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            // add resource
            ShowLoadingSpinner("Adding resource...");

            bool success = _libraryService.AddResource(
                fields["Title"],
                fields["Author/Creator"],
                year,
                fields["Genre/Category"],
                resourceType
            );

            if (success)
            {
                ShowMessage("✅ Resource added successfully!", SUCCESS_COLOR);
            }
            else
            {
                ShowMessage("❌ Failed to add resource. Please check your input.", ERROR_COLOR);
            }

            WaitForKeyPress();
        }

        private void SearchResourcesMenu()
        {
            string[] searchOptions =
            {
                "🔤 Search by Title",
                "👤 Search by Author",
                "🏷️  Search by Genre",
                "📅 Search by Year Range",
                "📋 View All Resources",
                "🔙 Back to Main Menu",
            };

            while (true)
            {
                Console.Clear();
                DrawHeader("SEARCH LIBRARY");

                int choice = ShowSelectionMenu("Choose search type:", searchOptions);
                if (choice == -1 || choice == 5)
                    return;

                List<LibraryResource> results = new List<LibraryResource>();

                switch (choice)
                {
                    case 0:
                        string title = GetUserInput(
                            "Enter title (partial search supported):",
                            "text"
                        );
                        if (title != null)
                            results = _libraryService.SearchByTitlePartial(title);
                        break;

                    case 1:
                        string author = GetUserInput("Enter author name:", "text");
                        if (author != null)
                            results = _libraryService.SearchByAuthor(author);
                        break;

                    case 2:
                        string genre = GetUserInput("Enter genre:", "text");
                        if (genre != null)
                            results = _libraryService.SearchByGenre(genre);
                        break;

                    case 3:
                        string minYear = GetUserInput("Enter minimum year:", "number");
                        if (minYear == null)
                            break;
                        string maxYear = GetUserInput("Enter maximum year:", "number");
                        if (maxYear == null)
                            break;

                        if (
                            int.TryParse(minYear, out int min) && int.TryParse(maxYear, out int max)
                        )
                        {
                            results = _libraryService.SearchByYearRange(min, max);
                        }
                        break;

                    case 4:
                        results = _libraryService.GetAllResourcesSorted();
                        break;
                }

                if (results != null)
                {
                    DisplaySearchResults(results);
                }
            }
        }

        private void DisplaySearchResults(List<LibraryResource> resources)
        {
            Console.Clear();
            DrawHeader($"SEARCH RESULTS ({resources.Count} found)");

            if (!resources.Any())
            {
                ShowMessage("No resources found matching your criteria.", ConsoleColor.Yellow);
                WaitForKeyPress();
                return;
            }

            // paginated results
            int pageSize = 10;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)resources.Count / pageSize);

            while (true)
            {
                Console.Clear();
                DrawHeader($"SEARCH RESULTS (Page {currentPage + 1}/{totalPages})");

                var pageItems = resources.Skip(currentPage * pageSize).Take(pageSize).ToList();

                DrawBox(Console.WindowWidth - 4, pageItems.Count + 2);

                for (int i = 0; i < pageItems.Count; i++)
                {
                    var resource = pageItems[i];
                    Console.SetCursorPosition(3, 6 + i);

                    string status = resource.IsAvailable ? "✅" : "❌";
                    string type = resource.Type.ToString().PadRight(8);

                    Console.ForegroundColor = resource.IsAvailable ? SUCCESS_COLOR : ERROR_COLOR;
                    Console.Write($"{status} ");
                    Console.ForegroundColor = TEXT_COLOR;
                    Console.Write($"[{resource.Id.ToString().PadLeft(3)}] ");
                    Console.ForegroundColor = PRIMARY_COLOR;
                    Console.Write($"{resource.Title.PadRight(30)} ");
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write($"by {resource.Author.PadRight(20)} ");
                    Console.ForegroundColor = ACCENT_COLOR;
                    Console.Write($"({resource.PublicationYear}) ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"{type} - {resource.Genre}");
                }

                // navigation help
                Console.SetCursorPosition(0, Console.WindowHeight - 4);
                CenterText("← Previous Page | → Next Page | ESC Back", ConsoleColor.DarkGray);

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (currentPage > 0)
                            currentPage--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (currentPage < totalPages - 1)
                            currentPage++;
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private void BorrowResourceMenu()
        {
            Console.Clear();
            DrawHeader("BORROW RESOURCE");

            string resourceIdStr = GetUserInput("Enter Resource ID:", "number");
            if (resourceIdStr == null)
                return;

            if (!int.TryParse(resourceIdStr, out int resourceId))
            {
                ShowMessage("Invalid Resource ID format!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            var resource = _libraryService.GetResourceById(resourceId);
            if (resource == null)
            {
                ShowMessage("Resource not found!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            if (!resource.IsAvailable)
            {
                ShowMessage("Resource is currently not available!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            // show resource details
            Console.Clear();
            DrawHeader("CONFIRM BORROWING");
            DrawResourceCard(resource);

            string borrowerName = GetUserInput("Enter your name:", "text");
            if (borrowerName == null)
                return;

            ShowLoadingSpinner("Processing borrowing request...");

            if (_libraryService.BorrowResource(resourceId, borrowerName))
            {
                ShowMessage("✅ Resource borrowed successfully!", SUCCESS_COLOR);
                ShowMessage($"Due date: {DateTime.Now.AddDays(14):dd/MM/yyyy}", ACCENT_COLOR);
            }
            else
            {
                ShowMessage("❌ Failed to borrow resource!", ERROR_COLOR);
            }

            WaitForKeyPress();
        }

        private void ReturnResourceMenu()
        {
            Console.Clear();
            DrawHeader("RETURN RESOURCE");

            string resourceIdStr = GetUserInput("Enter Resource ID:", "number");
            if (resourceIdStr == null)
                return;

            if (!int.TryParse(resourceIdStr, out int resourceId))
            {
                ShowMessage("Invalid Resource ID format!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            var resource = _libraryService.GetResourceById(resourceId);
            if (resource == null)
            {
                ShowMessage("Resource not found!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            if (resource.IsAvailable)
            {
                ShowMessage("Resource is not currently borrowed!", ERROR_COLOR);
                WaitForKeyPress();
                return;
            }

            ShowLoadingSpinner("Processing return...");

            if (_libraryService.ReturnResource(resourceId))
            {
                ShowMessage("✅ Resource returned successfully!", SUCCESS_COLOR);
            }
            else
            {
                ShowMessage("❌ Failed to return resource!", ERROR_COLOR);
            }

            WaitForKeyPress();
        }

        private void ViewReportsMenu()
        {
            string[] reportOptions =
            {
                "⚠️  Overdue Items",
                "📚 Currently Borrowed",
                "📊 Resources by Category",
                "📈 Library Statistics",
                "🔙 Back to Main Menu",
            };

            while (true)
            {
                Console.Clear();
                DrawHeader("REPORTS DASHBOARD");

                int choice = ShowSelectionMenu("Select report type:", reportOptions);
                if (choice == -1 || choice == 4)
                    return;

                switch (choice)
                {
                    case 0:
                        ShowOverdueReport();
                        break;
                    case 1:
                        ShowBorrowedReport();
                        break;
                    case 2:
                        ShowCategoryReport();
                        break;
                    case 3:
                        ShowStatisticsReport();
                        break;
                }
            }
        }

        private void ManageResourcesMenu()
        {
            string[] manageOptions =
            {
                "✏️  Update Resource",
                "🗑️  Remove Resource",
                "🔙 Back to Main Menu",
            };

            while (true)
            {
                Console.Clear();
                DrawHeader("MANAGE RESOURCES");

                int choice = ShowSelectionMenu("Select action:", manageOptions);
                if (choice == -1 || choice == 2)
                    return;

                switch (choice)
                {
                    case 0:
                        UpdateResourceMenu();
                        break;
                    case 1:
                        RemoveResourceMenu();
                        break;
                }
            }
        }

        // ui helper methods
        private void DrawHeader(string title)
        {
            Console.SetCursorPosition(0, 1);
            CenterText("═══════════════════════════════════════", PRIMARY_COLOR);
            CenterText($"  {title}  ", ACCENT_COLOR);
            CenterText("═══════════════════════════════════════", PRIMARY_COLOR);
        }

        private void DrawMenuBox(string[] items)
        {
            int boxWidth = 50;
            int boxHeight = items.Length + 4;
            int startX = (Console.WindowWidth - boxWidth) / 2;
            int startY = 8;

            // draw box
            DrawBox(boxWidth, boxHeight, startX, startY);

            // draw menu items
            for (int i = 0; i < items.Length; i++)
            {
                Console.SetCursorPosition(startX + 3, startY + 2 + i);

                if (i == _selectedIndex)
                {
                    Console.BackgroundColor = SELECTED_COLOR;
                    Console.ForegroundColor = BACKGROUND_COLOR;
                    Console.Write($"► {items[i].PadRight(boxWidth - 7)}");
                    Console.ResetColor();
                    Console.BackgroundColor = BACKGROUND_COLOR;
                    Console.ForegroundColor = TEXT_COLOR;
                }
                else
                {
                    Console.ForegroundColor = TEXT_COLOR;
                    Console.Write($"  {items[i]}");
                }
            }

            // navigation help
            Console.SetCursorPosition(0, Console.WindowHeight - 3);
            CenterText("↑↓ Navigate | ENTER Select | ESC Exit", ConsoleColor.DarkGray);
        }

        private void DrawBox(int width, int height, int startX = -1, int startY = -1)
        {
            if (startX == -1)
                startX = (Console.WindowWidth - width) / 2;
            if (startY == -1)
                startY = 5;

            Console.ForegroundColor = PRIMARY_COLOR;

            // top border
            Console.SetCursorPosition(startX, startY);
            Console.Write("╔" + new string('═', width - 2) + "╗");

            // side borders
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(startX, startY + i);
                Console.Write("║");
                Console.SetCursorPosition(startX + width - 1, startY + i);
                Console.Write("║");
            }

            // bottom border
            Console.SetCursorPosition(startX, startY + height - 1);
            Console.Write("╚" + new string('═', width - 2) + "╝");

            Console.ForegroundColor = TEXT_COLOR;
        }

        private void DrawResourceCard(LibraryResource resource)
        {
            DrawBox(60, 8);

            Console.SetCursorPosition((Console.WindowWidth - 56) / 2, 7);
            Console.ForegroundColor = PRIMARY_COLOR;
            Console.WriteLine($"ID: {resource.Id}");

            Console.SetCursorPosition((Console.WindowWidth - 56) / 2, 8);
            Console.ForegroundColor = ACCENT_COLOR;
            Console.WriteLine($"Title: {resource.Title}");

            Console.SetCursorPosition((Console.WindowWidth - 56) / 2, 9);
            Console.ForegroundColor = TEXT_COLOR;
            Console.WriteLine($"Author: {resource.Author}");

            Console.SetCursorPosition((Console.WindowWidth - 56) / 2, 10);
            Console.WriteLine($"Year: {resource.PublicationYear} | Genre: {resource.Genre}");

            Console.SetCursorPosition((Console.WindowWidth - 56) / 2, 11);
            Console.ForegroundColor = resource.IsAvailable ? SUCCESS_COLOR : ERROR_COLOR;
            Console.WriteLine($"Status: {(resource.IsAvailable ? "Available" : "Borrowed")}");
        }

        private int ShowSelectionMenu(string prompt, string[] options)
        {
            int selected = 0;

            while (true)
            {
                Console.SetCursorPosition(0, 6);
                CenterText(prompt, ACCENT_COLOR);

                DrawBox(50, options.Length + 4);

                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition((Console.WindowWidth - 46) / 2, 9 + i);

                    if (i == selected)
                    {
                        Console.BackgroundColor = SELECTED_COLOR;
                        Console.ForegroundColor = BACKGROUND_COLOR;
                        Console.Write($"► {options[i].PadRight(42)}");
                        Console.ResetColor();
                        Console.BackgroundColor = BACKGROUND_COLOR;
                        Console.ForegroundColor = TEXT_COLOR;
                    }
                    else
                    {
                        Console.Write($"  {options[i]}");
                    }
                }

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selected = (selected - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        selected = (selected + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        return selected;

                    case ConsoleKey.Escape:
                        return -1;
                }
            }
        }

        private string GetUserInput(string prompt, string type = "text")
        {
            Console.Clear();
            DrawHeader("INPUT REQUIRED");

            Console.SetCursorPosition(0, 8);
            CenterText(prompt, ACCENT_COLOR);

            DrawBox(60, 5);
            Console.SetCursorPosition((Console.WindowWidth - 56) / 2, 12);
            Console.ForegroundColor = PRIMARY_COLOR;
            Console.Write("► ");
            Console.ForegroundColor = TEXT_COLOR;

            Console.CursorVisible = true;
            string input = Console.ReadLine()?.Trim();
            Console.CursorVisible = false;

            if (string.IsNullOrEmpty(input))
            {
                ShowMessage("Input cannot be empty!", ERROR_COLOR);
                WaitForKeyPress();
                return GetUserInput(prompt, type);
            }

            return input;
        }

        private void ShowLoadingSpinner(string message)
        {
            string[] spinner = { "⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏" };

            Console.Clear();
            DrawHeader("PROCESSING");

            for (int i = 0; i < 15; i++)
            {
                Console.SetCursorPosition(0, Console.WindowHeight / 2);
                CenterText($"{spinner[i % spinner.Length]} {message}", PRIMARY_COLOR);
                Thread.Sleep(100);
            }
        }

        private void ShowMessage(string message, ConsoleColor color)
        {
            Console.SetCursorPosition(0, Console.WindowHeight / 2 + 2);
            CenterText(message, color);
        }

        private void CenterText(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
            Console.WriteLine(text);
            Console.ForegroundColor = TEXT_COLOR;
        }

        private void WaitForKeyPress()
        {
            Console.SetCursorPosition(0, Console.WindowHeight - 2);
            CenterText("Press any key to continue...", ConsoleColor.DarkGray);
            Console.ReadKey(true);
        }

        private void ShowExitAnimation()
        {
            Console.Clear();
            Console.SetCursorPosition(0, Console.WindowHeight / 2);
            CenterText("Thank you for using Librarian! 📚", ACCENT_COLOR);
            CenterText("Goodbye! ✨", PRIMARY_COLOR);
            Thread.Sleep(1500);
            Console.Clear();
        }

        // placeholder methods for reports (keeping original functionality)
        private void ShowOverdueReport()
        {
            Console.Clear();
            DrawHeader("OVERDUE ITEMS REPORT");

            var overdueItems = _libraryService.GetOverdueResources();

            if (!overdueItems.Any())
            {
                ShowMessage("✅ No overdue items found!", SUCCESS_COLOR);
            }
            else
            {
                ShowMessage($"⚠️ {overdueItems.Count} overdue items found", ERROR_COLOR);
                // display logic here
            }

            WaitForKeyPress();
        }

        private void ShowBorrowedReport()
        {
            Console.Clear();
            DrawHeader("CURRENTLY BORROWED ITEMS");

            var borrowedItems = _libraryService.GetBorrowedResources();
            ShowMessage($"📚 {borrowedItems.Count} items currently borrowed", PRIMARY_COLOR);

            WaitForKeyPress();
        }

        private void ShowCategoryReport()
        {
            Console.Clear();
            DrawHeader("RESOURCES BY CATEGORY");

            var categories = _libraryService.GetResourcesByCategory();
            // display category stats

            WaitForKeyPress();
        }

        private void ShowStatisticsReport()
        {
            Console.Clear();
            DrawHeader("LIBRARY STATISTICS");

            int total = _libraryService.GetTotalResourceCount();
            int available = _libraryService.GetAvailableResourceCount();

            ShowMessage(
                $"📊 Total: {total} | Available: {available} | Borrowed: {total - available}",
                PRIMARY_COLOR
            );

            WaitForKeyPress();
        }

        private void UpdateResourceMenu()
        {
            // placeholder - implement update logic
            ShowMessage("Update functionality coming soon!", ACCENT_COLOR);
            WaitForKeyPress();
        }

        private void RemoveResourceMenu()
        {
            // placeholder - implement remove logic
            ShowMessage("Remove functionality coming soon!", ACCENT_COLOR);
            WaitForKeyPress();
        }
    }
}
