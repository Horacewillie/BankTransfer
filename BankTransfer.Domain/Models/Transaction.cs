using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class Transaction
    {
        public Transaction(string? transactionReference, decimal amount, Status transferStatus, string? receipent)
        {
            TransactionReference = transactionReference;
            Amount = amount;
            TransferStatus = Status.Pending;
            Receipent = receipent;
        }

        public Guid Id { get; set; }

        public string? TransactionReference { get; set; }

        public decimal Amount { get; set; }

        public Status TransferStatus { get; set; }

        public string? Receipent { get; set; }
    }

    public enum Status
    {
        Pending,
        Success,
        Failed
    }
}
