using System;

namespace Librarian.Models
{
    public class BorrowRecord
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }
        public string BorrowerName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }

        // navigation property
        public LibraryResource Resource { get; set; }

        public BorrowRecord()
        {
            BorrowDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14); // default 2 week loan
            IsReturned = false;
        }

        public BorrowRecord(int resourceId, string borrowerName)
        {
            ResourceId = resourceId;
            BorrowerName = borrowerName;
            BorrowDate = DateTime.Now;
            DueDate = DateTime.Now.AddDays(14);
            IsReturned = false;
        }

        public bool IsOverdue()
        {
            return !IsReturned && DateTime.Now > DueDate;
        }

        public int DaysOverdue()
        {
            if (!IsOverdue())
                return 0;
            return (DateTime.Now - DueDate).Days;
        }

        public override string ToString()
        {
            string status =
                IsReturned ? "Returned"
                : IsOverdue() ? $"OVERDUE ({DaysOverdue()} days)"
                : "Active";
            return $"Borrowed by {BorrowerName} on {BorrowDate:dd/MM/yyyy}, Due: {DueDate:dd/MM/yyyy} - {status}";
        }
    }
}
