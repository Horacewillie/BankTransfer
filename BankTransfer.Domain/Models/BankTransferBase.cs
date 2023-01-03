using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class BankTransferBase
    {
        public decimal? Amount { get; set; }
        public string? BeneficiaryAccountNumber { get; set; }
        public string? BeneficiaryAccountName { get; set; }
        public string? BeneficiaryBankCode { get; set; }
        public string? TransactionReference { get; set; }
        public string? CurrencyCode { get; set; }
    }
}
