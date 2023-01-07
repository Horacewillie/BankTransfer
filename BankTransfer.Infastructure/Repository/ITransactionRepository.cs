using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Infastructure.Repository
{
    public interface ITransactionRepository
    {
        void AddTransaction(Transaction transaction);

        Task<Transaction> FindTransaction(Guid transactionId);

        Task<int> SaveChanges(CancellationToken cancellationToken = default, bool clearChangeTracker = false);
    }
}
