using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }

        public string? TransactionReference { get; set; }

        public decimal Amount { get; set; }

        public string? Receipent { get; set; }
    }
}
