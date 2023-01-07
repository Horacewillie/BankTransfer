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
        Task<ApiResponse<List<BankInfo>>> GetBanks(ClientConfig config);

        Task<ApiResponse<AccountInfo>> ValidateAccountNumber(ClientConfig config, ValidateAccountNumberQuery query);

        Task<ApiResponse<TransferResponse>> InitiateBankTransfer(ClientConfig config, BankTransferRequest query);

        Task<ApiResponse<TransactionStatusResponse>> StatusOfTransaction(ClientConfig config, string transactionReference);

        Task HandleBankTransfer(BankTransferMessage bankTransferMessage);
    }
}
