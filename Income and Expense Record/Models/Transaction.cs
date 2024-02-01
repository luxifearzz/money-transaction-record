using System.ComponentModel.DataAnnotations;

namespace Income_and_Expense_Record.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public string Label { get; set; }
    }
}
