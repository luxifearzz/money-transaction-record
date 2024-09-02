namespace Income_and_Expense_Record.Models
{
    public class Filter
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string? Name { get; set; }

        public string? Label { get; set; }

        public string? OrderBy { get; set; }

        public bool IsDesc { get; set; }
    }
}
