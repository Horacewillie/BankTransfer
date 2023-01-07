using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Configuration
{
    public class ServiceBusConfig
    {
        public const string Config_Key = "ServiceBusConfig";
        public string? BankTransferConnection { get; set; }
    }
}
