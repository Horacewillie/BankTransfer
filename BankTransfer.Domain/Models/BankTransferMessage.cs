using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class BankTransferMessage : QueueMessage
    {
        public decimal Amount { get; set; }

        public override string QueueName => "myqueue";

        public override string Label => "unknown";

        public Guid TransactionId { get; set; }

        public int MaxRetry { get; set; }
    }

    public class PayStackTransferMessage : BankTransferMessage
    {
       public override string Label => "paystack";
    }

    public class FlutterwaveTransferMessage : BankTransferMessage
    {
        public override string Label => "flutterwave";
    }

}
