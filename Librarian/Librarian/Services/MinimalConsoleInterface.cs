using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Librarian.Models;
using Librarian.Services;

namespace Librarian.Services
{
    public class MinimalConsoleInterface
    {
        private readonly LibraryService _libraryService;
        private bool _running;
        private int _selectedIndex = 0;
        private int _lastWindowWidth = 0;

        // minimal color palette
        private const ConsoleColor ACCENT = ConsoleColor.DarkCyan;
        private const ConsoleColor SUCCESS = ConsoleColor.DarkGreen;
        private const ConsoleColor ERROR = ConsoleColor.DarkRed;
        private const ConsoleColor SUBTLE = ConsoleColor.DarkGray;
        private const ConsoleColor TEXT = ConsoleColor.Gray;
        private const ConsoleColor BRIGHT = ConsoleColor.White;

        public MinimalConsoleInterface()
        {
            _libraryService = new LibraryService();
            _running = true;

            Console.CursorVisible = false;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = TEXT;
            Console.Clear();
        }

        public void Run()
        {
            ShowWelcome();

            while (_running)
            {
                ShowMainMenu();
            }

            ShowGoodbye();
            _libraryService.Dispose();
        }

        private void ShowWelcome()
        {
            // removed the glitchy welcome screen flash
            // system initializes directly to main menu for better UX
        }

        private void ShowMainMenu()
        {
            var options = new[]
            {
                "add resource",
                "search library",
                "read content",
                "borrow item",
                "return item",
                "view reports",
                "library members",
                "manage resources",
                "exit",
            };

            _lastWindowWidth = GetSafeWindowWidth(); // initialize

            while (true)
            {
                // check for window resize and force redraw if needed
                int currentWidth = GetSafeWindowWidth();
                if (_lastWindowWidth != currentWidth)
                {
                    _lastWindowWidth = currentWidth;
                    // force a complete redraw on resize
                    Thread.Sleep(50); // small delay to avoid race conditions
                }

                Console.Clear();

                // add vertical spacing for centering
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                // centered title (similar to opencode)
                CenterWrite("LIBRARY SYSTEM", ACCENT);
                CenterWrite("modern digital library management", SUBTLE);

                Console.WriteLine();
                Console.WriteLine();

                // centered menu options with proper containerization
                for (int i = 0; i < options.Length; i++)
                {
                    int consoleWidth = GetSafeWindowWidth();
                    int maxContentWidth = Math.Min(100, consoleWidth - 20);
                    int minPadding = 10;

                    string option = options[i];

                    // truncate option if too long
                    if (option.Length > maxContentWidth - 2) // account for selector
                    {
                        option = option.Substring(0, maxContentWidth - 5) + "...";
                    }

                    string selector = i == _selectedIndex ? "›" : " ";
                    string fullText = $"{selector} {option}";
                    int totalPadding = consoleWidth - fullText.Length;
                    int leftPadding = Math.Max(minPadding, totalPadding / 2);
                    string padding = new string(' ', leftPadding);

                    Console.Write(padding);

                    if (i == _selectedIndex)
                    {
                        Console.ForegroundColor = ACCENT;
                        Console.Write("› ");
                        Console.ForegroundColor = BRIGHT;
                    }
                    else
                    {
                        Console.ForegroundColor = SUBTLE;
                        Console.Write("  ");
                        Console.ForegroundColor = TEXT;
                    }

                    Console.WriteLine(option);
                }

                Console.WriteLine();
                CenterWrite("↑↓ navigate  enter select  esc exit", SUBTLE);

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        _selectedIndex = (_selectedIndex - 1 + options.Length) % options.Length;
                        break;

                    case ConsoleKey.DownArrow:
                        _selectedIndex = (_selectedIndex + 1) % options.Length;
                        break;

                    case ConsoleKey.Enter:
                        ExecuteAction(_selectedIndex);
                        if (!_running)
                            return;
                        break;

                    case ConsoleKey.Escape:
                        _running = false;
                        return;
                }
            }
        }

        private void ExecuteAction(int index)
        {
            switch (index)
            {
                case 0:
                    AddResource();
                    break;
                case 1:
                    SearchLibrary();
                    break;
                case 2:
                    ReadContent();
                    break;
                case 3:
                    BorrowItem();
                    break;
                case 4:
                    ReturnItem();
                    break;
                case 5:
                    ViewReports();
                    break;
                case 6:
                    ViewLibraryMembers();
                    break;
                case 7:
                    ManageResources();
                    break;
                case 8:
                    _running = false;
                    break;
            }
        }

        private void AddResource()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  ADD RESOURCE");
            Console.WriteLine();

            var title = GetInput("title");
            if (title == null)
                return;

            var author = GetInput("author");
            if (author == null)
                return;

            var yearStr = GetInput("year");
            if (yearStr == null)
                return;

            if (!int.TryParse(yearStr, out int year))
            {
                ShowError("invalid year format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            var genre = GetInput("genre");
            if (genre == null)
                return;

            var types = new[] { "book", "journal", "media" };
            var typeIndex = ShowSelection("resource type", types);
            if (typeIndex == -1)
                return;

            ShowLoading("adding resource");

            if (_libraryService.AddResource(title, author, year, genre, (ResourceType)typeIndex))
            {
                ShowSuccess("resource added successfully");
            }
            else
            {
                ShowError("failed to add resource");
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private void SearchLibrary()
        {
            var options = new[]
            {
                "search by title",
                "search by author",
                "search by genre",
                "search by year",
                "view all resources",
                "back",
            };

            while (true)
            {
                Console.Clear();
                Console.WriteLine();

                WriteAccent("  SEARCH LIBRARY");
                Console.WriteLine();

                var choice = ShowSelection("search type", options);
                if (choice == -1 || choice == 5)
                    return;

                List<LibraryResource> results = new List<LibraryResource>();

                switch (choice)
                {
                    case 0:
                        var title = GetInput("title");
                        if (title != null)
                            results = _libraryService.SearchByTitlePartial(title);
                        break;

                    case 1:
                        var author = GetInput("author");
                        if (author != null)
                            results = _libraryService.SearchByAuthor(author);
                        break;

                    case 2:
                        var genre = GetInput("genre");
                        if (genre != null)
                            results = _libraryService.SearchByGenre(genre);
                        break;

                    case 3:
                        var minYear = GetInput("min year");
                        if (minYear == null)
                            break;
                        var maxYear = GetInput("max year");
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
                    ShowResults(results);
                }
            }
        }

        private void ShowResults(List<LibraryResource> resources)
        {
            if (!resources.Any())
            {
                Console.Clear();
                Console.WriteLine();
                WriteAccent("  search results");
                WriteSubtle("  no resources found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            // interactive resource browser with pagination
            int selectedIndex = 0;
            int itemsPerPage = 8;
            int currentPage = 0;
            int totalPages = (int)Math.Ceiling((double)resources.Count / itemsPerPage);

            while (true)
            {
                Console.Clear();

                // add vertical spacing for centering
                Console.WriteLine();
                Console.WriteLine();

                // centered header with pagination info
                CenterWrite("SEARCH RESULTS", ACCENT);
                CenterWrite(
                    $"found {resources.Count} items • page {currentPage + 1} of {totalPages}",
                    SUBTLE
                );

                Console.WriteLine();
                Console.WriteLine();

                // calculate which resources to display on current page
                int startIndex = currentPage * itemsPerPage;
                int endIndex = Math.Min(startIndex + itemsPerPage, resources.Count);
                var displayResources = resources.Skip(startIndex).Take(itemsPerPage).ToList();

                for (int i = 0; i < displayResources.Count; i++)
                {
                    var resource = displayResources[i];
                    int consoleWidth = GetSafeWindowWidth();
                    int maxContentWidth = Math.Min(100, consoleWidth - 20);
                    int minPadding = 10;

                    // build the resource line
                    string status = resource.IsAvailable ? "•" : "×";
                    string content = resource.HasReadableContent() ? " •" : "";
                    string typeIndicator = $"[{resource.Type}]";
                    string resourceLine =
                        $"{status} [{resource.Id:D3}] {typeIndicator} {resource.Title} - {resource.Author} ({resource.PublicationYear}){content}";

                    // truncate if too long
                    if (resourceLine.Length > maxContentWidth - 2) // account for selector
                    {
                        resourceLine = resourceLine.Substring(0, maxContentWidth - 5) + "...";
                    }

                    string selector = i == selectedIndex ? "›" : " ";
                    string fullText = $"{selector} {resourceLine}";
                    int totalPadding = consoleWidth - fullText.Length;
                    int leftPadding = Math.Max(minPadding, totalPadding / 2);
                    string padding = new string(' ', leftPadding);

                    Console.Write(padding);

                    if (i == selectedIndex)
                    {
                        Console.ForegroundColor = ACCENT;
                        Console.Write("› ");
                        Console.ForegroundColor = resource.IsAvailable ? SUCCESS : ERROR;
                        Console.Write(status);
                        Console.ForegroundColor = BRIGHT;
                    }
                    else
                    {
                        Console.ForegroundColor = SUBTLE;
                        Console.Write("  ");
                        Console.ForegroundColor = resource.IsAvailable ? SUCCESS : ERROR;
                        Console.Write(status);
                        Console.ForegroundColor = TEXT;
                    }

                    Console.Write($" [{resource.Id:D3}] ");
                    
                    // add colored type indicator
                    Console.ForegroundColor = SUBTLE;
                    Console.Write($"[{resource.Type}] ");
                    Console.ForegroundColor = i == selectedIndex ? BRIGHT : TEXT;
                    
                    Console.Write($"{resource.Title} - {resource.Author} ({resource.PublicationYear})");

                    if (resource.HasReadableContent())
                    {
                        Console.ForegroundColor = ACCENT;
                        Console.Write(" •");
                    }

                    Console.WriteLine();
                    Console.ResetColor();
                }

                Console.WriteLine();
                Console.WriteLine();

                // navigation help with pagination
                if (totalPages > 1)
                {
                    CenterWrite("↑↓ navigate  enter view details  ←→ page  esc back", SUBTLE);
                }
                else
                {
                    CenterWrite("↑↓ navigate  enter view details  esc back", SUBTLE);
                }

                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex =
                            selectedIndex > 0 ? selectedIndex - 1 : displayResources.Count - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex =
                            selectedIndex < displayResources.Count - 1 ? selectedIndex + 1 : 0;
                        break;

                    case ConsoleKey.LeftArrow:
                        if (totalPages > 1)
                        {
                            currentPage = currentPage > 0 ? currentPage - 1 : totalPages - 1;
                            selectedIndex = 0; // reset selection on page change
                        }
                        break;

                    case ConsoleKey.RightArrow:
                        if (totalPages > 1)
                        {
                            currentPage = currentPage < totalPages - 1 ? currentPage + 1 : 0;
                            selectedIndex = 0; // reset selection on page change
                        }
                        break;

                    case ConsoleKey.Enter:
                        ShowResourceDetails(displayResources[selectedIndex]);
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }
        }

        private void ShowResourceDetails(LibraryResource resource)
        {
            Console.Clear();

            // add vertical spacing for centering
            Console.WriteLine();
            Console.WriteLine();

            // centered resource details
            CenterWrite(resource.Title.ToUpper(), ACCENT);
            CenterWrite($"by {resource.Author}", SUBTLE);
            CenterWrite($"{resource.Type} • {resource.Genre} • {resource.PublicationYear}", SUBTLE);

            Console.WriteLine();

            if (!string.IsNullOrEmpty(resource.Description))
            {
                // center multi-line description
                var descLines = resource.Description.Split('\n');
                foreach (var line in descLines)
                {
                    CenterWrite(line.Trim(), TEXT);
                }
                Console.WriteLine();
            }

            // centered status info
            string statusText = resource.IsAvailable ? "AVAILABLE" : "CURRENTLY BORROWED";
            ConsoleColor statusColor = resource.IsAvailable ? SUCCESS : ERROR;
            CenterWrite(statusText, statusColor);

            if (resource.HasReadableContent())
            {
                CenterWrite("• readable content available", ACCENT);
            }

            Console.WriteLine();
            Console.WriteLine();
            CenterWrite("press any key to return", SUBTLE);

            Console.ReadKey(true);
        }

        private void BorrowItem()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  BORROW ITEM");
            Console.WriteLine();

            // show all available resource IDs
            ShowResourceIdReference(readableContentOnly: false);
            Console.WriteLine();

            var idStr = GetInput("resource id");
            if (idStr == null)
                return;

            if (!int.TryParse(idStr, out int id))
            {
                ShowError("invalid id format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            var resource = _libraryService.GetResourceById(id);
            if (resource == null)
            {
                ShowError("resource not found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            if (!resource.IsAvailable)
            {
                ShowError("resource not available");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            Console.WriteLine();
            Console.Write("  ");
            Console.ForegroundColor = BRIGHT;
            Console.WriteLine(resource.Title);
            Console.Write("  ");
            Console.ForegroundColor = SUBTLE;
            Console.WriteLine($"by {resource.Author} ({resource.PublicationYear})");
            Console.WriteLine();

            var borrower = GetInput("your name");
            if (borrower == null)
                return;

            ShowLoading("processing request");

            if (_libraryService.BorrowResource(id, borrower))
            {
                ShowSuccess("item borrowed successfully");
                WriteSubtle($"  due: {DateTime.Now.AddDays(14):MMM dd, yyyy}");
            }
            else
            {
                ShowError("failed to borrow item");
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private void ReturnItem()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  RETURN ITEM");
            Console.WriteLine();

            // show all resource IDs
            ShowResourceIdReference(readableContentOnly: false);
            Console.WriteLine();

            var idStr = GetInput("resource id");
            if (idStr == null)
                return;

            if (!int.TryParse(idStr, out int id))
            {
                ShowError("invalid id format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            ShowLoading("processing return");

            if (_libraryService.ReturnResource(id))
            {
                ShowSuccess("item returned successfully");
            }
            else
            {
                ShowError("failed to return item");
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private void ViewReports()
        {
            var options = new[] { "overdue items", "borrowed items", "library stats", "borrowing history", "back" };

            while (true)
            {
                Console.Clear();
                Console.WriteLine();

                WriteAccent("  VIEW REPORTS");
                Console.WriteLine();

                var choice = ShowSelection("report type", options);
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
                        ShowStatsReport();
                        break;
                    case 3:
                        ShowBorrowingHistoryReport();
                        break;
                }
            }
        }

        private void ShowOverdueReport()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  OVERDUE ITEMS");
            Console.WriteLine();

            var overdue = _libraryService.GetOverdueResources();

            if (!overdue.Any())
            {
                WriteSuccess("  no overdue items");
            }
            else
            {
                WriteSubtle($"  {overdue.Count} items overdue");
                Console.WriteLine();

                foreach (var item in overdue.Take(5))
                {
                    Console.Write("  ");
                    Console.ForegroundColor = ERROR;
                    Console.Write("!");
                    Console.ForegroundColor = TEXT;
                    Console.Write($" [{item.Resource.Id:D3}] ");
                    Console.ForegroundColor = BRIGHT;
                    Console.Write(item.Resource.Title);
                    Console.ForegroundColor = SUBTLE;
                    Console.WriteLine($" ({item.DaysOverdue()} days overdue)");
                }

                if (overdue.Count > 5)
                {
                    Console.WriteLine();
                    WriteSubtle($"  ... and {overdue.Count - 5} more overdue items");
                }
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private void ShowBorrowedReport()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  BORROWED ITEMS");
            Console.WriteLine();

            var borrowed = _libraryService.GetBorrowedResources();
            WriteSubtle($"  {borrowed.Count} items currently borrowed");

            if (borrowed.Any())
            {
                Console.WriteLine();
                foreach (var item in borrowed.Take(8))
                {
                    Console.Write("  ");
                    Console.ForegroundColor = ACCENT;
                    Console.Write("!");
                    Console.ForegroundColor = TEXT;
                    Console.Write($" [{item.Resource.Id:D3}] ");
                    Console.ForegroundColor = BRIGHT;
                    Console.Write(item.Resource.Title);
                    Console.ForegroundColor = SUBTLE;
                    Console.Write($" - {item.BorrowerName}");
                    Console.ForegroundColor = TEXT;
                    Console.WriteLine($" (due {item.DueDate:MMM dd})");
                }

                if (borrowed.Count > 8)
                {
                    Console.WriteLine();
                    WriteSubtle($"  ... and {borrowed.Count - 8} more borrowed items");
                }
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private void ShowStatsReport()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  LIBRARY STATISTICS");
            Console.WriteLine();

            var total = _libraryService.GetTotalResourceCount();
            var available = _libraryService.GetAvailableResourceCount();
            var borrowed = total - available;
            var overdue = _libraryService.GetOverdueResources().Count;

            Console.Write("  ");
            Console.ForegroundColor = BRIGHT;
            Console.Write($"{total}");
            Console.ForegroundColor = TEXT;
            Console.WriteLine(" total resources");

            Console.Write("  ");
            Console.ForegroundColor = SUCCESS;
            Console.Write($"{available}");
            Console.ForegroundColor = TEXT;
            Console.WriteLine(" available");

            Console.Write("  ");
            Console.ForegroundColor = ACCENT;
            Console.Write($"{borrowed}");
            Console.ForegroundColor = TEXT;
            Console.WriteLine(" borrowed");

            if (overdue > 0)
            {
                Console.Write("  ");
                Console.ForegroundColor = ERROR;
                Console.Write($"{overdue}");
                Console.ForegroundColor = TEXT;
                Console.WriteLine(" overdue");
            }

            Console.WriteLine();
            if (total > 0)
            {
                var utilization = (double)borrowed / total * 100;
                Console.Write("  ");
                Console.ForegroundColor = SUBTLE;
                Console.WriteLine($"utilization: {utilization:F1}%");
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private string GetUserEmail(string name)
        {
            // automatically add @bunnyrabb.it for known users
            var knownUsers = new[] { "josh", "ashu", "rigon", "alex" };
            string cleanName = name.ToLower().Trim();
            
            if (knownUsers.Contains(cleanName))
            {
                return $"{cleanName}@bunnyrabb.it";
            }
            
            return $"{cleanName}@bunnyrabb.it"; // default to @bunnyrabb.it for all users
        }

        private void ShowBorrowingHistoryReport()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  BORROWING HISTORY");
            Console.WriteLine();

            var history = _libraryService.GetBorrowingHistory();

            if (!history.Any())
            {
                WriteSubtle("  no borrowing history found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            WriteSubtle($"  showing {history.Count} borrowing records (most recent first)");
            Console.WriteLine();

            foreach (var record in history)
            {
                // resource info
                Console.Write("  ");
                Console.ForegroundColor = BRIGHT;
                Console.Write($"[{record.Resource.Id:D3}] ");
                Console.ForegroundColor = SUBTLE;
                Console.Write($"[{record.Resource.Type}] ");
                Console.ForegroundColor = TEXT;
                Console.WriteLine($"{record.Resource.Title} by {record.Resource.Author}");

                // borrower info
                Console.Write("      ");
                Console.ForegroundColor = ACCENT;
                Console.Write($"{record.BorrowerName}");
                Console.ForegroundColor = SUBTLE;
                Console.Write(" • ");
                Console.ForegroundColor = TEXT;
                Console.WriteLine(GetUserEmail(record.BorrowerName));

                // dates info
                Console.Write("      ");
                Console.ForegroundColor = SUBTLE;
                Console.Write("borrowed: ");
                Console.ForegroundColor = TEXT;
                Console.Write($"{record.BorrowDate:dd/MM/yyyy}");
                
                Console.ForegroundColor = SUBTLE;
                Console.Write(" • due: ");
                Console.ForegroundColor = TEXT;
                Console.Write($"{record.DueDate:dd/MM/yyyy}");
                
                if (record.IsReturned && record.ReturnDate.HasValue)
                {
                    Console.ForegroundColor = SUBTLE;
                    Console.Write(" • returned: ");
                    Console.ForegroundColor = SUCCESS;
                    Console.Write($"{record.ReturnDate.Value:dd/MM/yyyy}");
                }
                else if (record.IsOverdue())
                {
                    Console.ForegroundColor = SUBTLE;
                    Console.Write(" • ");
                    Console.ForegroundColor = ERROR;
                    Console.Write($"OVERDUE ({record.DaysOverdue()} days)");
                }
                else
                {
                    Console.ForegroundColor = SUBTLE;
                    Console.Write(" • ");
                    Console.ForegroundColor = ACCENT;
                    Console.Write("currently borrowed");
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.ResetColor();
            }

            ShowNavigationHelp();
            WaitForKey();
        }

        private void ViewLibraryMembers()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  LIBRARY MEMBERS");
            Console.WriteLine();

            var memberStats = _libraryService.GetLibraryMemberStats();

            if (!memberStats.Any())
            {
                WriteSubtle("  no library members found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            WriteSubtle($"  showing {memberStats.Count} active members");
            Console.WriteLine();

            foreach (var member in memberStats)
            {
                // member name and email
                Console.Write("  ");
                Console.ForegroundColor = BRIGHT;
                Console.Write($"{member.Name}");
                Console.ForegroundColor = SUBTLE;
                Console.Write(" • ");
                Console.ForegroundColor = TEXT;
                Console.WriteLine($"{member.Email}");

                // borrowing statistics
                Console.Write("      ");
                Console.ForegroundColor = ACCENT;
                Console.Write($"{member.TotalBorrowed}");
                Console.ForegroundColor = SUBTLE;
                Console.Write(" total borrowed");
                
                if (member.CurrentlyBorrowed > 0)
                {
                    Console.Write(" • ");
                    Console.ForegroundColor = ACCENT;
                    Console.Write($"{member.CurrentlyBorrowed}");
                    Console.ForegroundColor = SUBTLE;
                    Console.Write(" current");
                }

                if (member.Returned > 0)
                {
                    Console.Write(" • ");
                    Console.ForegroundColor = SUCCESS;
                    Console.Write($"{member.Returned}");
                    Console.ForegroundColor = SUBTLE;
                    Console.Write(" returned");
                }

                if (member.Overdue > 0)
                {
                    Console.Write(" • ");
                    Console.ForegroundColor = ERROR;
                    Console.Write($"{member.Overdue}");
                    Console.ForegroundColor = SUBTLE;
                    Console.Write(" overdue");
                }

                // last activity
                if (member.LastBorrowDate.HasValue)
                {
                    Console.WriteLine();
                    Console.Write("      ");
                    Console.ForegroundColor = SUBTLE;
                    Console.Write("last activity: ");
                    Console.ForegroundColor = TEXT;
                    Console.Write($"{member.LastBorrowDate.Value:dd/MM/yyyy}");
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.ResetColor();
            }

            ShowNavigationHelp();
            WaitForKey();
        }

        private void ShowResourceIdReference(bool readableContentOnly = false)
        {
            var allResources = _libraryService.GetAllResourcesSorted();
            
            var books = readableContentOnly 
                ? allResources.Where(r => r.Type == ResourceType.Book && r.HasReadableContent()).ToList()
                : allResources.Where(r => r.Type == ResourceType.Book).ToList();
                
            var journals = readableContentOnly 
                ? allResources.Where(r => r.Type == ResourceType.Journal && r.HasReadableContent()).ToList()
                : allResources.Where(r => r.Type == ResourceType.Journal).ToList();
                
            var media = readableContentOnly 
                ? allResources.Where(r => r.Type == ResourceType.Media && r.HasReadableContent()).ToList()
                : allResources.Where(r => r.Type == ResourceType.Media).ToList();

            WriteSubtle(readableContentOnly ? "  available content:" : "  resource ids:");
            Console.WriteLine();

            if (books.Any())
            {
                WriteSubtle("  books: ");
                Console.ForegroundColor = TEXT;
                Console.Write(string.Join(", ", books.Select(b => b.Id.ToString())));
                Console.WriteLine();
            }

            if (journals.Any())
            {
                WriteSubtle("  journals: ");
                Console.ForegroundColor = TEXT;
                Console.Write(string.Join(", ", journals.Select(j => j.Id.ToString())));
                Console.WriteLine();
            }

            if (media.Any())
            {
                WriteSubtle("  media: ");
                Console.ForegroundColor = TEXT;
                Console.Write(string.Join(", ", media.Select(m => m.Id.ToString())));
                Console.WriteLine();
            }

            Console.ResetColor();
        }

        private void ReadContent()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  READ CONTENT");
            Console.WriteLine();

            // show helpful ID reference lists for readable content
            ShowResourceIdReference(readableContentOnly: true);
            Console.WriteLine();

            var idStr = GetInput("resource id");
            if (idStr == null)
                return;

            if (!int.TryParse(idStr, out int id))
            {
                ShowError("invalid id format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            var resource = _libraryService.GetResourceById(id);
            if (resource == null)
            {
                ShowError("resource not found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            if (!resource.HasReadableContent())
            {
                ShowError("no readable content available");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            ShowContentReader(resource);
        }

        private void ShowContentReader(LibraryResource resource)
        {
            var readerName = GetInput("your name");
            if (readerName == null)
                return;

            var session = new ReadingSession(readerName, resource);
            var contentLines = resource.Content?.Split('\n') ?? new string[0];
            var linesPerPage = 12;

            while (true)
            {
                Console.Clear();

                // centered layout similar to opencode interface
                ShowCenteredReader(resource, session, contentLines, linesPerPage);

                var key = Console.ReadKey(true);

                switch (key.KeyChar)
                {
                    case 'n':
                    case 'N':
                        if (!session.NextPage())
                        {
                            if (session.IsCompleted)
                            {
                                ShowSuccess("reading completed!");
                                session.EndTime = DateTime.Now;
                                Console.WriteLine();
                                ShowNavigationHelp();
                                WaitForKey();
                                return;
                            }
                        }
                        break;

                    case 'p':
                    case 'P':
                        session.PreviousPage();
                        break;

                    case 'b':
                    case 'B':
                        session.SetBookmark();
                        ShowSuccess($"bookmark set at page {session.CurrentPage}");
                        Thread.Sleep(1000);
                        break;

                    case 'g':
                    case 'G':
                        if (session.GoToBookmark())
                        {
                            ShowSuccess($"jumped to bookmark at page {session.CurrentPage}");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            ShowError("no bookmark set");
                            Thread.Sleep(1000);
                        }
                        break;

                    case 'q':
                    case 'Q':
                        session.EndTime = DateTime.Now;
                        ShowSuccess("reading session saved");
                        Thread.Sleep(1000);
                        return;
                }
            }
        }

        private void ShowCenteredReader(
            LibraryResource resource,
            ReadingSession session,
            string[] contentLines,
            int linesPerPage
        )
        {
            int consoleWidth = GetSafeWindowWidth();
            int maxContentWidth = Math.Min(80, consoleWidth - 20); // max 80 chars, with proper padding
            int minPadding = 10;

            // add vertical spacing for centering
            Console.WriteLine();
            Console.WriteLine();

            // centered title section
            CenterWrite(resource.Title.ToUpper(), ACCENT);
            CenterWrite($"by {resource.Author}", SUBTLE);
            CenterWrite($"[{resource.Type}] • {resource.Genre} • {resource.PublicationYear}", SUBTLE);
            CenterWrite(
                $"page {session.CurrentPage}/{session.TotalPages} • {session.GetProgressPercentage():F1}% complete",
                SUBTLE
            );

            Console.WriteLine();
            Console.WriteLine();

            // content area with containerized text
            var startLine = (session.CurrentPage - 1) * linesPerPage;
            var endLine = Math.Min(startLine + linesPerPage, contentLines.Length);

            for (int i = startLine; i < endLine; i++)
            {
                if (i < contentLines.Length)
                {
                    Console.ForegroundColor = TEXT;
                    var line = contentLines[i];

                    // truncate line if too long
                    if (line.Length > maxContentWidth)
                    {
                        line = line.Substring(0, maxContentWidth - 3) + "...";
                    }

                    // center the line with proper padding
                    int totalPadding = consoleWidth - line.Length;
                    int leftPadding = Math.Max(minPadding, totalPadding / 2);
                    string padding = new string(' ', leftPadding);

                    Console.WriteLine(padding + line);
                }
            }

            // add some spacing
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine();
            }

            // centered navigation options (similar to opencode layout)
            CenterWrite("n", ACCENT, "next page", TEXT);
            CenterWrite("p", ACCENT, "previous page", TEXT);
            CenterWrite("b", ACCENT, "set bookmark", TEXT);
            CenterWrite("g", ACCENT, "go to bookmark", TEXT);
            CenterWrite("q", ERROR, "quit reading", TEXT);

            Console.WriteLine();
            Console.WriteLine();
        }

        private void CenterWrite(string text, ConsoleColor color)
        {
            int consoleWidth;
            try
            {
                consoleWidth = Console.WindowWidth;
            }
            catch
            {
                consoleWidth = 80; // fallback width during resize
            }

            int maxContentWidth = Math.Min(100, consoleWidth - 20); // max 100 chars, with 10 char padding on each side
            int minPadding = 10; // minimum padding from edges

            // truncate text if too long
            if (text.Length > maxContentWidth)
            {
                text = text.Substring(0, maxContentWidth - 3) + "...";
            }

            int totalPadding = consoleWidth - text.Length;
            int leftPadding = Math.Max(minPadding, totalPadding / 2);
            string padding = new string(' ', leftPadding);

            Console.ForegroundColor = color;
            Console.WriteLine(padding + text);
            Console.ResetColor();
        }

        private int GetSafeWindowWidth()
        {
            try
            {
                return Console.WindowWidth;
            }
            catch
            {
                return 80; // fallback width during resize operations
            }
        }

        private void CenterWrite(
            string command,
            ConsoleColor commandColor,
            string description,
            ConsoleColor descColor
        )
        {
            int consoleWidth = GetSafeWindowWidth();
            int maxContentWidth = Math.Min(100, consoleWidth - 20);
            int minPadding = 10;

            string fullText = $"{command}          {description}";

            // truncate description if too long
            if (fullText.Length > maxContentWidth)
            {
                int availableDescLength = maxContentWidth - command.Length - 13; // account for spacing and ellipsis
                if (availableDescLength > 0)
                {
                    description =
                        description.Substring(0, Math.Min(description.Length, availableDescLength))
                        + "...";
                    fullText = $"{command}          {description}";
                }
            }

            int totalPadding = consoleWidth - fullText.Length;
            int leftPadding = Math.Max(minPadding, totalPadding / 2);
            string padding = new string(' ', leftPadding);

            Console.Write(padding);
            Console.ForegroundColor = commandColor;
            Console.Write(command);
            Console.ForegroundColor = descColor;
            Console.WriteLine($"          {description}");
            Console.ResetColor();
        }

        private void ManageResources()
        {
            var options = new[] { "update resource", "remove resource", "back" };

            while (true)
            {
                Console.Clear();
                Console.WriteLine();

                WriteAccent("  MANAGE RESOURCES");
                Console.WriteLine();

                var choice = ShowSelection("action", options);
                if (choice == -1 || choice == 2)
                    return;

                switch (choice)
                {
                    case 0:
                        UpdateResource();
                        break;
                    case 1:
                        RemoveResource();
                        break;
                }
            }
        }

        private void UpdateResource()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  UPDATE RESOURCE");
            Console.WriteLine();

            // show all resource IDs
            ShowResourceIdReference(readableContentOnly: false);
            Console.WriteLine();

            var idStr = GetInput("resource id to update");
            if (idStr == null)
                return;

            if (!int.TryParse(idStr, out int id))
            {
                ShowError("invalid id format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            var resource = _libraryService.GetResourceById(id);
            if (resource == null)
            {
                ShowError("resource not found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            Console.WriteLine();
            Console.Write("  ");
            Console.ForegroundColor = BRIGHT;
            Console.WriteLine($"updating: {resource.Title} by {resource.Author}");
            Console.WriteLine();

            var title = GetInput($"new title (current: {resource.Title})");
            if (title == null)
                return;

            var author = GetInput($"new author (current: {resource.Author})");
            if (author == null)
                return;

            var yearStr = GetInput($"new year (current: {resource.PublicationYear})");
            if (yearStr == null)
                return;

            if (
                !int.TryParse(yearStr, out int year)
                || year < 1000
                || year > DateTime.Now.Year + 10
            )
            {
                ShowError("invalid year format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            var genre = GetInput($"new genre (current: {resource.Genre})");
            if (genre == null)
                return;

            ShowLoading("updating resource");

            if (_libraryService.UpdateResource(id, title, author, year, genre))
            {
                ShowSuccess("resource updated successfully");
            }
            else
            {
                ShowError("failed to update resource");
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        private void RemoveResource()
        {
            Console.Clear();
            Console.WriteLine();

            WriteAccent("  REMOVE RESOURCE");
            Console.WriteLine();

            // show all resource IDs
            ShowResourceIdReference(readableContentOnly: false);
            Console.WriteLine();

            var idStr = GetInput("resource id to remove");
            if (idStr == null)
                return;

            if (!int.TryParse(idStr, out int id))
            {
                ShowError("invalid id format");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            var resource = _libraryService.GetResourceById(id);
            if (resource == null)
            {
                ShowError("resource not found");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            Console.WriteLine();
            Console.Write("  ");
            Console.ForegroundColor = ERROR;
            Console.WriteLine($"warning: this will permanently delete:");
            Console.Write("  ");
            Console.ForegroundColor = BRIGHT;
            Console.WriteLine(
                $"{resource.Title} by {resource.Author} ({resource.PublicationYear})"
            );
            Console.WriteLine();

            Console.Write("  ");
            Console.ForegroundColor = SUBTLE;
            Console.WriteLine("type 'DELETE' to confirm or press esc to cancel");
            Console.WriteLine();

            var confirmation = GetInput("confirmation");
            if (confirmation == null)
                return;

            if (confirmation != "DELETE")
            {
                ShowError("deletion cancelled - confirmation text must be exactly 'DELETE'");
                Console.WriteLine();
                ShowNavigationHelp();
                WaitForKey();
                return;
            }

            ShowLoading("removing resource");

            if (_libraryService.RemoveResource(id))
            {
                ShowSuccess("resource removed successfully");
            }
            else
            {
                ShowError("failed to remove resource - it may be currently borrowed");
            }

            Console.WriteLine();
            ShowNavigationHelp();
            WaitForKey();
        }

        // helper methods
        private string GetInput(string prompt)
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            WriteAccent($"{prompt}");
            Console.WriteLine();
            WriteSubtle("type your input and press enter");
            WriteSubtle("press esc to cancel");
            Console.WriteLine();

            // center the input prompt
            int consoleWidth = GetSafeWindowWidth();
            int minPadding = 10;
            string inputPrompt = "› ";
            int leftPadding = Math.Max(minPadding, (consoleWidth - inputPrompt.Length) / 2);
            string padding = new string(' ', leftPadding);

            Console.Write(padding);
            Console.ForegroundColor = SUBTLE;
            Console.Write("› ");
            Console.ForegroundColor = BRIGHT;
            Console.CursorVisible = true;

            string input = "";
            ConsoleKeyInfo keyInfo;

            do
            {
                keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.Escape)
                {
                    Console.CursorVisible = false;
                    return null;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0)
                {
                    input = input.Substring(0, input.Length - 1);
                    Console.Write("\b \b");
                }
                else if (
                    keyInfo.Key != ConsoleKey.Enter
                    && keyInfo.Key != ConsoleKey.Backspace
                    && !char.IsControl(keyInfo.KeyChar)
                )
                {
                    input += keyInfo.KeyChar;
                    Console.Write(keyInfo.KeyChar);
                }
            } while (keyInfo.Key != ConsoleKey.Enter);

            Console.CursorVisible = false;

            if (string.IsNullOrWhiteSpace(input))
            {
                ShowError("input required");
                Thread.Sleep(1000);
                return GetInput(prompt);
            }

            return input.Trim();
        }

        private int ShowSelection(string prompt, string[] options)
        {
            int selected = 0;

            while (true)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();

                CenterWrite(prompt.ToUpper(), ACCENT);
                Console.WriteLine();
                Console.WriteLine();

                // centered menu options with proper containerization
                for (int i = 0; i < options.Length; i++)
                {
                    int consoleWidth = GetSafeWindowWidth();
                    int maxContentWidth = Math.Min(100, consoleWidth - 20);
                    int minPadding = 10;

                    string option = options[i];

                    // truncate option if too long
                    if (option.Length > maxContentWidth - 2) // account for selector
                    {
                        option = option.Substring(0, maxContentWidth - 5) + "...";
                    }

                    string selector = i == selected ? "›" : " ";
                    string fullText = $"{selector} {option}";
                    int totalPadding = consoleWidth - fullText.Length;
                    int leftPadding = Math.Max(minPadding, totalPadding / 2);
                    string padding = new string(' ', leftPadding);

                    Console.Write(padding);

                    if (i == selected)
                    {
                        Console.ForegroundColor = BRIGHT;
                        Console.Write("› ");
                        Console.ForegroundColor = BRIGHT;
                    }
                    else
                    {
                        Console.ForegroundColor = SUBTLE;
                        Console.Write("  ");
                        Console.ForegroundColor = TEXT;
                    }

                    Console.WriteLine(option);
                }

                Console.WriteLine();
                CenterWrite("↑↓ navigate  enter select  esc back", SUBTLE);

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

        private void ShowLoading(string message)
        {
            Console.WriteLine();
            CenterWrite($"● {message}", SUBTLE);
            Thread.Sleep(800);
        }

        private void ShowSuccess(string message)
        {
            Console.WriteLine();
            CenterWrite($"● {message}", SUCCESS);
        }

        private void ShowError(string message)
        {
            Console.WriteLine();
            CenterWrite($"● {message}", ERROR);
        }

        private void WriteAccent(string text)
        {
            // remove leading spaces and use centered layout
            CenterWrite(text.TrimStart(), ACCENT);
        }

        private void WriteSubtle(string text)
        {
            // remove leading spaces and use centered layout
            CenterWrite(text.TrimStart(), SUBTLE);
        }

        private void WriteSuccess(string text)
        {
            // remove leading spaces and use centered layout
            CenterWrite(text.TrimStart(), SUCCESS);
        }

        private void ShowNavigationHelp()
        {
            WriteSubtle("esc back  any key continue");
        }

        private void WaitForKey()
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                return; // let the calling method handle the back navigation
            }
        }

        private void ShowGoodbye()
        {
            Console.Clear();
            Console.WriteLine();
            WriteAccent("  SYSTEM CLOSING");
            WriteSubtle("  thank you for using the library system");
            Console.WriteLine();
            Thread.Sleep(1200);
            Console.Clear();
        }
    }
}
