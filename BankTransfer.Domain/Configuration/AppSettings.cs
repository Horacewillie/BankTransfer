using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Configuration
{
    public class AppSettings
    {
        public const string Config_Key = "AppSettings";

        public string? ApiKey { get; set; }
    }
}
