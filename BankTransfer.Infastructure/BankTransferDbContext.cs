using BankTransfer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace BankTransfer.Infastructure
{
    public class BankTransferDbContext :DbContext
    {

        public BankTransferDbContext(DbContextOptions<BankTransferDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TransactionConfig());
        }

        public DbSet<Transaction> Transaction { get; set; }

    }
}