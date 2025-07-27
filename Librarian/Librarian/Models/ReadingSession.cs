using System;

namespace Librarian.Models
{
    public class ReadingSession
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public LibraryResource Resource { get; set; }
        public string ReaderName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool IsCompleted { get; set; }
        public int? BookmarkPosition { get; set; }
        public string? Notes { get; set; }

        public ReadingSession()
        {
            StartTime = DateTime.Now;
            CurrentPage = 1;
            IsCompleted = false;
        }

        public ReadingSession(string readerName, LibraryResource resource)
        {
            ReaderName = readerName;
            ResourceId = resource.Id;
            Resource = resource;
            StartTime = DateTime.Now;
            CurrentPage = 1;
            TotalPages = CalculateActualPages(resource);
            IsCompleted = false;
        }

        // calculate actual pages based on content length
        private int CalculateActualPages(LibraryResource resource)
        {
            if (string.IsNullOrEmpty(resource.Content))
                return 1;

            var contentLines = resource.Content.Split('\n');
            int linesPerPage = 12; // matches what we use in the reader interface
            int actualPages = (int)Math.Ceiling((double)contentLines.Length / linesPerPage);

            return Math.Max(1, actualPages); // ensure at least 1 page
        }

        // calculate reading progress percentage
        public double GetProgressPercentage()
        {
            if (TotalPages <= 0)
                return 0;
            return (CurrentPage * 100.0) / TotalPages;
        }

        // calculate reading duration
        public TimeSpan GetReadingDuration()
        {
            if (EndTime.HasValue)
                return EndTime.Value - StartTime;
            else
                return DateTime.Now - StartTime;
        }

        // method to advance to next page/section
        public bool NextPage()
        {
            if (CurrentPage < TotalPages)
            {
                CurrentPage++;
                if (CurrentPage >= TotalPages)
                    IsCompleted = true;
                return true;
            }
            return false;
        }

        // method to go to previous page/section
        public bool PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
                IsCompleted = false;
                return true;
            }
            return false;
        }

        // method to set bookmark
        public void SetBookmark()
        {
            BookmarkPosition = CurrentPage;
        }

        // method to go to bookmark
        public bool GoToBookmark()
        {
            if (
                BookmarkPosition.HasValue
                && BookmarkPosition.Value > 0
                && BookmarkPosition.Value <= TotalPages
            )
            {
                CurrentPage = BookmarkPosition.Value;
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            var duration = GetReadingDuration();
            var progress = GetProgressPercentage();
            var status = IsCompleted ? "Completed" : "In Progress";

            return $"{ReaderName} - {Resource?.Title} - Page {CurrentPage}/{TotalPages} ({progress:F1}%) - {status} - Duration: {duration:hh\\:mm}";
        }
    }
}
