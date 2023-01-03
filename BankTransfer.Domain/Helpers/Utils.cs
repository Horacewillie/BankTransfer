using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Helpers
{
    public static class Utils
    {
        public static string? GenerateTransactionRefernce()
        {
            Guid guid = Guid.NewGuid();

            // Convert the GUID to a string with the format "N"
            string uuid = guid.ToString("N");

            // Truncate the string to 16 digits
            string uuid16 = uuid.Substring(0, 16);

            return uuid16;
        }
    }
}
