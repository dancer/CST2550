# Library Management System

M01031195
Console-based Library Management System for CST2550 coursework. Built with C# .NET 8.0 and SQL Server LocalDB.

## Features

- **Resource Management**: Add, search, and manage books, journals, and media
- **Interactive Reading**: Read books with bookmarks and save progress
- **Search System**: Find resources by title, author, genre, or year ranges
- **Borrowing Workflow**: Track borrowed items and due dates
- **Reports**: View statistics, overdue items, and borrowing history
- **Custom Data Structures**: Hash tables and binary search trees built from scratch

## How to run it

### What you need first

1. **SQL Server LocalDB** (this is required - the app won't work without it)
2. **.NET 8.0 SDK** or higher

### Getting SQL Server LocalDB

**Easiest way (if you have Visual Studio):**

- Open Visual Studio
- Go to Tools → Get Tools and Features
- Click "Individual Components" tab
- Search for "SQL Server Express LocalDB"
- Install it

**If you don't have Visual Studio:**

- Download from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- Choose "Express" → "Custom" installation
- Select "LocalDB" component

**Command line (if you're into that):**

```bash
winget install Microsoft.SQLServerExpressLocalDB
```

### Actually running it

1. **Open a terminal and go to the project folder:**

   ```bash
   cd Librarian\Librarian
   ```

2. **Run it:**

   ```bash
   dotnet run
   ```

   The database gets created automatically the first time you run it (pretty cool!)

### Running tests

```bash
cd Librarian\Librarian.Tests
dotnet test
```

### Building everything

If you need to build the whole thing:

```bash
dotnet build
```

If packages are missing:

```bash
dotnet restore
```

## How to use it

### Main menu stuff

- **add resource** - Add new books, journals, or media to the library
- **search library** - Find stuff by title, author, genre, whatever
- **read content** - Actually read the books with bookmarks (this was fun to build)
- **borrow item** - Check out resources to people
- **return item** - Return stuff when you're done
- **view reports** - See statistics, what's overdue, what's borrowed
- **manage resources** - Update or delete resources
- **exit** - Close the app

### Reading books

The reading feature is pretty cool:

1. Enter a resource ID (try 1-5 for the sample books I wrote)
2. Enter your name to start reading
3. Navigate with these keys:
   - **n** - Next page
   - **p** - Previous page
   - **b** - Set a bookmark
   - **g** - Go to your bookmark
   - **q** - Quit and save where you left off

### What's actually in the library

**Books with full readable content:**

- **ID 1**: The Young Wizard's Journey
- **ID 2**: Heroes United: The Battle for Earth
- **ID 3**: Wizarding World: Magical Creatures Encyclopedia
- **ID 5**: The Dark Arts: A History of Forbidden Magic
- **ID 11**: The Hobbit's Great Adventure
- **ID 12**: Pride and Modern Prejudice
- **ID 13**: 1984: A Dystopian Future
- **ID 14**: To Kill a Mockingbird's Legacy
- **ID 15**: The Great Gatsby's Era

**Academic Journals:**

- **ID 6**: Journal of Advanced Magical Theory
- **ID 7**: Modern Defense Strategies Quarterly
- **ID 16**: Journal of Computer Science Research
- **ID 17**: International Medical Research Quarterly

**Movies/Media on disc:**

- **ID 4**: Superhero Training Academy: Complete Season 1
- **ID 8**: The Avengers
- **ID 9**: The Dark Knight
- **ID 10**: Inception
- **ID 18**: The Matrix
- **ID 19**: Interstellar

## Technologies

- **.NET 8.0** with C#
- **Entity Framework Core** with SQL Server LocalDB
- **xUnit** for testing
- **Custom data structures** (no built-in collections)

## Database

Uses SQL Server LocalDB with Entity Framework Code First. Database gets created automatically on first run.

- **Database Name**: `LibraryDB`
- **Manual setup script**: `Librarian\Database\CreateLibraryDatabase.sql`

**If database issues occur:**

1. Close the app
2. Delete database files from LocalDB location
3. Run app again to recreate with sample data

## Project Structure

```
Librarian/
├── Librarian/                  # Main application
│   ├── Models/                 # Data models
│   ├── DataStructures/         # Custom hash table & BST
│   ├── Services/               # Business logic & UI
│   └── Data/                   # EF Core context
├── Librarian.Tests/            # Unit tests
├── Database/                   # SQL setup script
└── REPORT.md                   # Development report
```

## Troubleshooting

**Database issues:**

- Ensure SQL Server LocalDB is installed
- Database recreates automatically if deleted

**App won't start:**

- Check you're in `Librarian\Librarian` folder
- Verify .NET 8.0 SDK is installed
- Run `dotnet restore` if packages are missing

## About

Built for CST2550 Coursework

This is an academic project for educational purposes only.
