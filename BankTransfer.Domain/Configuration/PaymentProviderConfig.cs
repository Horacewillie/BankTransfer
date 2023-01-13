using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Configuration
{
    public class PaymentProviderOptions
    {

        public const string Config_Key = "PaymentProviderOptions";

        public string? DefaultProvider { get; set; }
        public List<ClientConfig> PaymentProviderConfigs { get; set; }


        public string GetPaymentProvider(string? provider)
        {
            var paymentProvider = PaymentProviderConfigs?.FirstOrDefault(c => string.Equals(c.Name, provider, StringComparison.OrdinalIgnoreCase));
            if(paymentProvider == null)
                return DefaultProvider!;
            return paymentProvider.Name!;
        }
    }

    public class ClientConfig
    {
        public string? Name { get; set; }
        public string? ProviderApiKey { get; set; }
        public string? GenerateReceipientUrl { get; set; }
        public string Currency { get; set; }

        public string? GetTransactionStatusUrl { get; set; }
        public string? TransferUrl { get; set; }
        //public string? FlutterwaveApiKey { get; set; }
        public string? GetAllBanksUrl { get; set; }
        public string? ValidateAccountNumberUrl { get; set; }
    }
}
