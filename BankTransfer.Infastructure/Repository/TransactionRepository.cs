using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Infastructure.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        protected BankTransferDbContext DbContext;
        public TransactionRepository(BankTransferDbContext context)
        {
            DbContext = context ?? throw new ArgumentNullException(nameof(context));
        }
        public void AddTransaction(Transaction transaction)
        {
            DbContext.Add(transaction);
        }

        public async Task<Transaction> FindTransaction(Guid transactionId)
        {
            return await DbContext!.Transaction!.FindAsync(transactionId);
        }


        public async Task<int> SaveChanges(CancellationToken cancellationToken = default,
            bool clearChangeTracker = false)
        {
            var result = await DbContext.SaveChangesAsync(cancellationToken);
            if (clearChangeTracker)
                ClearChangeTracker();
            return result;
        }

        private void ClearChangeTracker()
        {
            foreach (var entry in DbContext.ChangeTracker.Entries())
            {
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            }
        }


    }
}
