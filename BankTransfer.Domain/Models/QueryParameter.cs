using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BankTransfer.Domain.Models
{
    public class ProviderQuery
    {
        public string? Provider { get; set; }
    }

    public class ValidateAccountNumberQuery : ProviderQuery
    {
        [FromQuery(Name = "account_number")]
        public string AccountNumber { get; set; }
        [FromQuery(Name = "bank_code")]
        public string Code { get; set; }
    }
}
