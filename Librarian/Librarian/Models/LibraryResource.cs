using System;

namespace Librarian.Models
{
    public enum ResourceType
    {
        Book,
        Journal,
        Media,
    }

    public class LibraryResource
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string? ISBN { get; set; }
        public int PublicationYear { get; set; }
        public string Genre { get; set; }
        public string? Publisher { get; set; }
        public int? PageCount { get; set; }
        public string Language { get; set; } = "English";
        public bool IsAvailable { get; set; }
        public ResourceType Type { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; } // full readable content
        public string? ContentPreview { get; set; } // first few paragraphs
        public string? CoverImagePath { get; set; }
        public decimal? Rating { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        public LibraryResource()
        {
            IsAvailable = true;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }

        public LibraryResource(
            string title,
            string author,
            int year,
            string genre,
            ResourceType type
        )
        {
            Title = title;
            Author = author;
            PublicationYear = year;
            Genre = genre;
            Type = type;
            IsAvailable = true;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }

        // method to check if resource has readable content
        public bool HasReadableContent()
        {
            return !string.IsNullOrEmpty(Content);
        }

        // method to get content preview
        public string GetPreview()
        {
            if (!string.IsNullOrEmpty(ContentPreview))
                return ContentPreview;

            if (!string.IsNullOrEmpty(Content) && Content.Length > 200)
                return Content.Substring(0, 200) + "...";

            return Content ?? "No preview available.";
        }

        public override string ToString()
        {
            string status = IsAvailable ? "Available" : "Borrowed";
            return $"[{Id}] {Title} by {Author} ({PublicationYear}) - {Genre} [{Type}] - {status}";
        }
    }
}
