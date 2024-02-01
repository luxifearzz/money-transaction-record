using Income_and_Expense_Record.Models;
using Microsoft.EntityFrameworkCore;

namespace Income_and_Expense_Record.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<Money> Moneys { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
