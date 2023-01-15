using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class Transaction : DbGuidEntity
    {
        public Transaction(string? transactionReference, decimal? amount, Status transferStatus, string? receipent)
        {
            TransactionReference = transactionReference;
            Amount = amount;
            TransferStatus = Status.Pending;
            Recepient = receipent!;
        }
        public Transaction(string? transactionReference, decimal? amount, Status transferStatus)
        {
            TransactionReference = transactionReference;
            Amount = amount;
            TransferStatus = Status.Pending;
        }

        public string? TransactionReference { get; set; }

        public decimal? Amount { get; set; }

        public Status TransferStatus { get; set; }

        public string Recepient { get; set; }
    }

    public enum Status
    {
        Pending,
        Success,
        Failed
    }
}
