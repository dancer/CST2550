using Librarian.Models;
using Xunit;

namespace Librarian.Tests
{
    public class LibraryResourceTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateResource()
        {
            var resource = new LibraryResource(
                "Test Book",
                "Test Author",
                2023,
                "Fiction",
                ResourceType.Book
            );

            Assert.Equal("Test Book", resource.Title);
            Assert.Equal("Test Author", resource.Author);
            Assert.Equal(2023, resource.PublicationYear);
            Assert.Equal("Fiction", resource.Genre);
            Assert.Equal(ResourceType.Book, resource.Type);
            Assert.True(resource.IsAvailable);
        }

        [Fact]
        public void DefaultConstructor_ShouldSetDefaultValues()
        {
            var resource = new LibraryResource();

            Assert.True(resource.IsAvailable);
            Assert.True(resource.CreatedDate <= DateTime.Now);
        }

        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            var resource = new LibraryResource(
                "1984",
                "George Orwell",
                1949,
                "Fiction",
                ResourceType.Book
            )
            {
                Id = 1,
            };

            string result = resource.ToString();

            Assert.Contains("1984", result);
            Assert.Contains("George Orwell", result);
            Assert.Contains("1949", result);
            Assert.Contains("Fiction", result);
            Assert.Contains("Available", result);
        }
    }

    public class BorrowRecordTests
    {
        [Fact]
        public void Constructor_ValidParameters_ShouldCreateRecord()
        {
            var record = new BorrowRecord(1, "John Doe");

            Assert.Equal(1, record.ResourceId);
            Assert.Equal("John Doe", record.BorrowerName);
            Assert.True(record.BorrowDate <= DateTime.Now);
            Assert.True(record.DueDate > record.BorrowDate);
            Assert.False(record.IsReturned);
        }

        [Fact]
        public void IsOverdue_PastDueDate_ShouldReturnTrue()
        {
            var record = new BorrowRecord(1, "Jane Doe")
            {
                DueDate = DateTime.Now.AddDays(-1),
                IsReturned = false,
            };

            Assert.True(record.IsOverdue());
        }

        [Fact]
        public void IsOverdue_NotPastDueDate_ShouldReturnFalse()
        {
            var record = new BorrowRecord(1, "John Smith")
            {
                DueDate = DateTime.Now.AddDays(7),
                IsReturned = false,
            };

            Assert.False(record.IsOverdue());
        }

        [Fact]
        public void IsOverdue_ReturnedItem_ShouldReturnFalse()
        {
            var record = new BorrowRecord(1, "Alice Brown")
            {
                DueDate = DateTime.Now.AddDays(-5),
                IsReturned = true,
            };

            Assert.False(record.IsOverdue());
        }

        [Fact]
        public void DaysOverdue_OverdueItem_ShouldReturnPositiveNumber()
        {
            var record = new BorrowRecord(1, "Bob Wilson")
            {
                DueDate = DateTime.Now.AddDays(-3),
                IsReturned = false,
            };

            int daysOverdue = record.DaysOverdue();

            Assert.True(daysOverdue >= 3);
        }
    }
}
