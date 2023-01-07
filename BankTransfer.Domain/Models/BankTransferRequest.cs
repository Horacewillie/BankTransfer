using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{

        public class BankTransferRequest : BankTransferBase
        {
            public string? Narration { get; set; }
            public int MaxRetryAttempt => 3;
            public string? CallbackUrl { get; set; }
            public string? Provider { get; set; }
        }
}
