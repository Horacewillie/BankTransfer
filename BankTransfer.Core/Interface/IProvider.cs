using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.Interface
{
    public interface IProvider
    {
        Task<List<BankInfo>> GetBanks(ClientConfig config);

        Task<AccountInfo> ValidateAccountNumber(ClientConfig config, ValidateAccountNumberQuery query);

        Task<object> InitiateBankTransfer(ClientConfig config, BankTransferRequest query);

        Task<TransactionStatusResponse> StatusOfTransaction(ClientConfig config, string transactionReference);

        Task HandleBankTransfer(BankTransferMessage bankTransferMessage);
    }
}
