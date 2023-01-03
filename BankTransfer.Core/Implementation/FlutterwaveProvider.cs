using BankTransfer.Core.Interface;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Exceptions;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;
using BankTransfer.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.Implementation
{
    public class FlutterwaveProvider : IProvider
    {
        private readonly ApiClient _apiClient;
        public FlutterwaveProvider(ApiClient client)
        {
            _apiClient = client;
        }
        public async Task<ApiResponse<List<BankInfo>>> GetBanks(ClientConfig config)
        {
            //Make request to the designated service provider
            var response = await _apiClient.Get<ApiResponse<List<BankDetail>>>(config?.GetAllBanksUrl!, config?.ProviderApiKey!);
            if (response.Data is null)
                throw new BadRequestException(response?.Message!);
            var result = response.Data
                .Select(s => new BankInfo { Code = s.Code, BankName = s.Name, LongName = s.Longcode })
                .ToList();

            return new ApiResponse<List<BankInfo>>() { Data = result, Message = response.Message, Status = response.Status };
        }

        public async Task<ApiResponse<TransferResponse>> InitiateBankTransfer(ClientConfig config, BankTransferRequest query)
        {
            var data = new
            {
                account_bank = query.BeneficiaryBankCode,
                account_number = query.BeneficiaryAccountNumber,
                amount = query.Amount,
                narration = query.Narration,
                currency = query.CurrencyCode,
                callback_url = query.CallbackUrl,
                reference = Utils.GenerateTransactionRefernce()
            };
            var response = await _apiClient.Post<ApiResponse<FlutterwaveTransferResponse>>(data, config?.TransferUrl!, config?.ProviderApiKey!, true, query.MaxRetryAttempt);
            if (response.Data is null)
                throw new BadRequestException(response?.Message!);
            return new ApiResponse<TransferResponse> { Data = MapToTransferResponse(response), Message = response.Message, Status = response.Status };
        }

        public async Task<ApiResponse<AccountInfo>> ValidateAccountNumber(ClientConfig config, ValidateAccountNumberQuery query)
        {
            var data = new { account_number = query.AccountNumber, account_bank = query.Code };
            //Make request
            var response = await _apiClient.Post<ApiResponse<AccountDetail>>(data, config?.ValidateAccountNumberUrl!, config?.ProviderApiKey!);
            var listOfBanks = await GetBanks(config!);
            var bank = listOfBanks!.Data!.Where(x => x.Code == query.Code)
                .SingleOrDefault();
            if (response.Data is null)
                throw new BadRequestException(response.Message!);

            var result = new AccountInfo
            {
                AccountName = response?.Data.Account_Name,
                AccountNumber = response?.Data.Account_Number,
                BankCode = bank!.Code,
                BankName = bank.BankName,
            };
            return new ApiResponse<AccountInfo> { Data = result, Message = response!.Message, Status = response.Status };
        }

        public async Task<ApiResponse<TransactionStatusResponse>> StatusOfTransaction(ClientConfig config, string transactionReference)
        {
            var id = transactionReference;
            var response = await _apiClient.Get<ApiResponse<FlutterwaveTransferResponse>>($"{config.GetTransactionStatusUrl}/{id}", config!.ProviderApiKey!);
            if (response.Data is null)
                throw new BadRequestException(response.Message!);

            return new ApiResponse<TransactionStatusResponse> { Data = MapToTransactionStatus(response), Status = response.Status, Message = response.Message };
        }

        private TransactionStatusResponse MapToTransactionStatus(ApiResponse<FlutterwaveTransferResponse> response)
        {
            var transferResponse = response?.Data;

            return new TransactionStatusResponse
            {
                Amount = transferResponse?.Amount,
                TransactionReference = transferResponse?.Reference,
                CurrencyCode = transferResponse?.Currency,
                BeneficiaryAccountName = transferResponse!.Full_Name,
                BeneficiaryAccountNumber = transferResponse.Account_Number,
                BeneficiaryBankCode = transferResponse.Bank_Code,
                TransactionDateTime = transferResponse.Created_at,
                ResponseMessage = response?.Message,
                Status = transferResponse?.Status,
                SessionId = string.Empty
            };
        }

        private static TransferResponse MapToTransferResponse(ApiResponse<FlutterwaveTransferResponse> response)
        {
            var transferResponse = response?.Data;
            return new TransferResponse
            {
                BeneficiaryBankCode = transferResponse!.Bank_Code,
                Amount = transferResponse.Amount,
                BeneficiaryAccountNumber = transferResponse.Account_Number,
                TransactionReference = transferResponse.Reference,
                Status = transferResponse.Status,
                ResponseCode = transferResponse.Is_Approved,
                CurrencyCode = transferResponse.Currency,
                BeneficiaryAccountName = transferResponse.Full_Name,
                ResponseMessage = response!.Message,
                TransactionDateTime = transferResponse.Created_at,
                SessionId = string.Empty
            };
        }
    }
}
