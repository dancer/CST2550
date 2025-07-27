using System;
using System.Collections.Generic;
using System.Linq;
using Librarian.Data;
using Librarian.DataStructures;
using Librarian.Models;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Services
{
    public class LibraryService
    {
        private readonly LibraryContext _context;
        private readonly ResourceSearchEngine _searchEngine;

        public LibraryService(LibraryContext context)
        {
            _context = context;
            _searchEngine = new ResourceSearchEngine();
            LoadResourcesIntoSearchEngine();
        }

        // parameterless constructor for direct database usage
        public LibraryService()
        {
            _context = new LibraryContext();
            _searchEngine = new ResourceSearchEngine();
            EnsureDatabaseCreated();
            LoadResourcesIntoSearchEngine();
        }

        // ensure database is created
        private void EnsureDatabaseCreated()
        {
            try
            {
                _context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"database initialization error: {ex.Message}");
            }
        }

        // load all resources from database into search engine
        private void LoadResourcesIntoSearchEngine()
        {
            try
            {
                var resources = _context.Resources.ToList();
                foreach (var resource in resources)
                {
                    _searchEngine.AddResource(resource);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error loading resources: {ex.Message}");
            }
        }

        // add new resource
        // time complexity: O(log n) due to search engine insertion
        public bool AddResource(
            string title,
            string author,
            int year,
            string genre,
            ResourceType type
        )
        {
            try
            {
                if (
                    string.IsNullOrWhiteSpace(title)
                    || string.IsNullOrWhiteSpace(author)
                    || string.IsNullOrWhiteSpace(genre)
                    || year < 1000
                    || year > DateTime.Now.Year + 1
                )
                {
                    return false;
                }

                var resource = new LibraryResource(
                    title.Trim(),
                    author.Trim(),
                    year,
                    genre.Trim(),
                    type
                );

                _context.Resources.Add(resource);
                _context.SaveChanges();

                // add to search engine after getting database id
                _searchEngine.AddResource(resource);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error adding resource: {ex.Message}");
                return false;
            }
        }

        // remove resource by id
        // time complexity: O(log n)
        public bool RemoveResource(int id)
        {
            try
            {
                var resource = _context.Resources.Find(id);
                if (resource == null)
                    return false;

                // check if resource is currently borrowed
                bool isBorrowed = _context.BorrowRecords.Any(br =>
                    br.ResourceId == id && !br.IsReturned
                );

                if (isBorrowed)
                {
                    Console.WriteLine("cannot remove resource - currently borrowed");
                    return false;
                }

                _context.Resources.Remove(resource);
                _context.SaveChanges();

                // remove from search engine
                _searchEngine.RemoveResource(id);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error removing resource: {ex.Message}");
                return false;
            }
        }

        // update resource details
        public bool UpdateResource(int id, string title, string author, int year, string genre)
        {
            try
            {
                var resource = _context.Resources.Find(id);
                if (resource == null)
                    return false;

                if (
                    string.IsNullOrWhiteSpace(title)
                    || string.IsNullOrWhiteSpace(author)
                    || string.IsNullOrWhiteSpace(genre)
                    || year < 1000
                    || year > DateTime.Now.Year + 1
                )
                {
                    return false;
                }

                // remove from search engine first
                _searchEngine.RemoveResource(id);

                // update database
                resource.Title = title.Trim();
                resource.Author = author.Trim();
                resource.PublicationYear = year;
                resource.Genre = genre.Trim();

                _context.SaveChanges();

                // re-add to search engine with updated info
                _searchEngine.AddResource(resource);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error updating resource: {ex.Message}");
                return false;
            }
        }

        // search by title
        // time complexity: O(1) average case
        public List<LibraryResource> SearchByTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return new List<LibraryResource>();
            return _searchEngine.SearchByTitle(title.Trim());
        }

        // search by partial title
        // time complexity: O(n)
        public List<LibraryResource> SearchByTitlePartial(string partialTitle)
        {
            if (string.IsNullOrWhiteSpace(partialTitle))
                return new List<LibraryResource>();
            return _searchEngine.SearchByTitlePartial(partialTitle.Trim());
        }

        // search by author
        // time complexity: O(1) average case
        public List<LibraryResource> SearchByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
                return new List<LibraryResource>();
            return _searchEngine.SearchByAuthor(author.Trim());
        }

        // search by genre
        // time complexity: O(1) average case
        public List<LibraryResource> SearchByGenre(string genre)
        {
            if (string.IsNullOrWhiteSpace(genre))
                return new List<LibraryResource>();
            return _searchEngine.SearchByGenre(genre.Trim());
        }

        // search by year range
        // time complexity: O(n)
        public List<LibraryResource> SearchByYearRange(int minYear, int maxYear)
        {
            return _searchEngine.SearchByYearRange(minYear, maxYear);
        }

        // get all resources sorted by title
        // time complexity: O(n)
        public List<LibraryResource> GetAllResourcesSorted()
        {
            return _searchEngine.GetAllResourcesSorted();
        }

        // get resource by id
        // time complexity: O(1)
        public LibraryResource GetResourceById(int id)
        {
            try
            {
                return _searchEngine.GetById(id);
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        // borrow resource
        public bool BorrowResource(int resourceId, string borrowerName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(borrowerName))
                    return false;

                var resource = _context.Resources.Find(resourceId);
                if (resource == null || !resource.IsAvailable)
                    return false;

                // create borrow record
                var borrowRecord = new BorrowRecord(resourceId, borrowerName.Trim());
                _context.BorrowRecords.Add(borrowRecord);

                // update resource availability
                resource.IsAvailable = false;
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error borrowing resource: {ex.Message}");
                return false;
            }
        }

        // return resource
        public bool ReturnResource(int resourceId)
        {
            try
            {
                var borrowRecord = _context.BorrowRecords.FirstOrDefault(br =>
                    br.ResourceId == resourceId && !br.IsReturned
                );

                if (borrowRecord == null)
                    return false;

                var resource = _context.Resources.Find(resourceId);
                if (resource == null)
                    return false;

                // update borrow record
                borrowRecord.ReturnDate = DateTime.Now;
                borrowRecord.IsReturned = true;

                // update resource availability
                resource.IsAvailable = true;
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error returning resource: {ex.Message}");
                return false;
            }
        }

        // get all borrowed resources
        public List<BorrowRecord> GetBorrowedResources()
        {
            try
            {
                return _context
                    .BorrowRecords.Include(br => br.Resource)
                    .Where(br => !br.IsReturned)
                    .OrderBy(br => br.DueDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting borrowed resources: {ex.Message}");
                return new List<BorrowRecord>();
            }
        }

        // get overdue resources
        public List<BorrowRecord> GetOverdueResources()
        {
            try
            {
                var today = DateTime.Now.Date;
                return _context
                    .BorrowRecords.Include(br => br.Resource)
                    .Where(br => !br.IsReturned && br.DueDate.Date < today)
                    .OrderBy(br => br.DueDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting overdue resources: {ex.Message}");
                return new List<BorrowRecord>();
            }
        }

        // get all borrowing history (both active and returned)
        public List<BorrowRecord> GetBorrowingHistory()
        {
            try
            {
                return _context
                    .BorrowRecords.Include(br => br.Resource)
                    .OrderByDescending(br => br.BorrowDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting borrowing history: {ex.Message}");
                return new List<BorrowRecord>();
            }
        }

        // get library member statistics
        public class MemberStats
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public int TotalBorrowed { get; set; }
            public int CurrentlyBorrowed { get; set; }
            public int Returned { get; set; }
            public int Overdue { get; set; }
            public DateTime? LastBorrowDate { get; set; }
        }

        public List<MemberStats> GetLibraryMemberStats()
        {
            try
            {
                var borrowRecords = _context.BorrowRecords.Include(br => br.Resource).ToList();

                var memberStats = borrowRecords
                    .GroupBy(br => br.BorrowerName)
                    .Select(group => new MemberStats
                    {
                        Name = group.Key,
                        Email = GetUserEmail(group.Key),
                        TotalBorrowed = group.Count(),
                        CurrentlyBorrowed = group.Count(br => !br.IsReturned && !br.IsOverdue()),
                        Returned = group.Count(br => br.IsReturned),
                        Overdue = group.Count(br => br.IsOverdue()),
                        LastBorrowDate = group.Max(br => br.BorrowDate),
                    })
                    .OrderByDescending(ms => ms.LastBorrowDate)
                    .ToList();

                return memberStats;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting member statistics: {ex.Message}");
                return new List<MemberStats>();
            }
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

        // get resources by category (genre)
        public Dictionary<string, int> GetResourcesByCategory()
        {
            try
            {
                return _context
                    .Resources.GroupBy(r => r.Genre)
                    .ToDictionary(g => g.Key, g => g.Count());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting resources by category: {ex.Message}");
                return new Dictionary<string, int>();
            }
        }

        // get total resource count
        public int GetTotalResourceCount()
        {
            try
            {
                return _context.Resources.Count();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting resource count: {ex.Message}");
                return 0;
            }
        }

        // get available resource count
        public int GetAvailableResourceCount()
        {
            try
            {
                return _context.Resources.Count(r => r.IsAvailable);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error getting available resource count: {ex.Message}");
                return 0;
            }
        }

        // dispose resources
        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
