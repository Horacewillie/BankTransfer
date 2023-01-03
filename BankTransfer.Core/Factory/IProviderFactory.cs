using BankTransfer.Core.Interface;
using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.Factory
{
    public interface IProviderFactory
    {
        IProvider GetProvider(ProviderEnum? provider);
    }
}
