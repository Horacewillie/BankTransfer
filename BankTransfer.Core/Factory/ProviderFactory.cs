using BankTransfer.Core.Implementation;
using BankTransfer.Core.Interface;
using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.Factory
{
    public class ProviderFactory : IProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public IProvider GetProvider(ProviderEnum? provider) => provider switch
        {
            ProviderEnum!.Flutterwave => (IProvider)_serviceProvider.GetService(typeof(FlutterwaveProvider)),

            _ => (IProvider)_serviceProvider.GetService(typeof(PaystackProvider))
        };
    }
}
