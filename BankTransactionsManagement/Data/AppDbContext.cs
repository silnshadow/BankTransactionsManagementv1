using Microsoft.EntityFrameworkCore;

namespace BankTransactionsManagement.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Example DbSet
        public DbSet<Transaction> Transactions { get; set; }
    }
}