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

        public override string QueueName => QueueConfigs.Transfer;

        public virtual string Label => "unknown";

        public Guid TransactionId { get; set; }

        public string? TransferUrl { get; set; }

        public string? ProviderApikey { get; set; }

        public int MaxRetry { get; set; }
    }

    public class PayStackTransferMessage : BankTransferMessage
    {
       public override string Label => QueueConfigs.PayStackLabel;
    }

    public class FlutterwaveTransferMessage : BankTransferMessage
    {
        public override string Label => QueueConfigs.FlutterwaveLabel;
    }

}
