using Librarian.Data;
using Librarian.Models;
using Librarian.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Librarian.Tests
{
    public class LibraryServiceTests : IDisposable
    {
        private readonly LibraryService _service;

        public LibraryServiceTests()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            var context = new LibraryContext(options);
            _service = new LibraryService(context);
        }

        [Fact]
        public void AddResource_ValidResource_ShouldReturnTrue()
        {
            bool result = _service.AddResource(
                "Test Book", 
                "Test Author", 
                2023, 
                "Fiction", 
                ResourceType.Book
            );

            Assert.True(result);
        }

        [Fact]
        public void SearchByTitle_ExistingTitle_ShouldReturnResource()
        {
            _service.AddResource("Unique Title", "Author", 2023, "Genre", ResourceType.Book);

            var results = _service.SearchByTitle("Unique Title");

            Assert.Single(results);
            Assert.Equal("Unique Title", results[0].Title);
        }

        [Fact]
        public void SearchByAuthor_ExistingAuthor_ShouldReturnResources()
        {
            _service.AddResource("Book 1", "Jane Smith", 2022, "Fiction", ResourceType.Book);
            _service.AddResource("Book 2", "Jane Smith", 2023, "Mystery", ResourceType.Book);

            var results = _service.SearchByAuthor("Jane Smith");

            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.Equal("Jane Smith", r.Author));
        }

        [Fact]
        public void BorrowResource_AvailableResource_ShouldReturnTrue()
        {
            _service.AddResource("Borrowable Book", "Author", 2023, "Genre", ResourceType.Book);
            var resources = _service.GetAllResourcesSorted();
            int resourceId = resources.First().Id;

            bool result = _service.BorrowResource(resourceId, "John Doe");

            Assert.True(result);
        }

        [Fact]
        public void BorrowResource_NonExistentResource_ShouldReturnFalse()
        {
            bool result = _service.BorrowResource(999, "John Doe");

            Assert.False(result);
        }

        [Fact]
        public void ReturnResource_BorrowedResource_ShouldReturnTrue()
        {
            _service.AddResource("Return Test Book", "Author", 2023, "Genre", ResourceType.Book);
            var resources = _service.GetAllResourcesSorted();
            int resourceId = resources.First().Id;
            
            _service.BorrowResource(resourceId, "Borrower");
            bool result = _service.ReturnResource(resourceId);

            Assert.True(result);
        }

        [Fact]
        public void GetAllResourcesSorted_ShouldReturnAllResources()
        {
            _service.AddResource("Test Book 1", "Author 1", 2023, "Genre 1", ResourceType.Book);
            _service.AddResource("Test Book 2", "Author 2", 2022, "Genre 2", ResourceType.Journal);

            var results = _service.GetAllResourcesSorted();

            Assert.True(results.Count >= 2);
        }

        [Fact] 
        public void SearchByGenre_ExistingGenre_ShouldReturnResources()
        {
            _service.AddResource("Sci-Fi Book 1", "Author 1", 2022, "Science Fiction", ResourceType.Book);
            _service.AddResource("Sci-Fi Book 2", "Author 2", 2023, "Science Fiction", ResourceType.Book);

            var results = _service.SearchByGenre("Science Fiction");

            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.Equal("Science Fiction", r.Genre));
        }

        [Fact]
        public void UpdateResource_ExistingResource_ShouldReturnTrue()
        {
            _service.AddResource("Original Title", "Original Author", 2020, "Original Genre", ResourceType.Book);
            var resources = _service.GetAllResourcesSorted();
            int resourceId = resources.First().Id;

            bool result = _service.UpdateResource(
                resourceId, 
                "Updated Title", 
                "Updated Author", 
                2023, 
                "Updated Genre"
            );

            Assert.True(result);
        }

        [Fact]
        public void RemoveResource_ExistingResource_ShouldReturnTrue()
        {
            _service.AddResource("To Delete", "Author", 2023, "Genre", ResourceType.Book);
            var resources = _service.GetAllResourcesSorted();
            int resourceId = resources.First().Id;

            bool result = _service.RemoveResource(resourceId);

            Assert.True(result);
        }

        public void Dispose()
        {
            _service?.Dispose();
        }
    }
}