using BankTransfer.Core.Interface;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Exceptions;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;
using BankTransfer.Infastructure.Repository;
using BankTransfer.Messaging;
using BankTransfer.Messaging.SignalRClient;
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
        private ITransactionRepository _transactionRepository;
        private readonly Messenger<FlutterwaveTransferMessage> _bankTransferMessenger;
        public FlutterwaveProvider(ApiClient client,
            ITransactionRepository transactionRepository, Messenger<FlutterwaveTransferMessage> bankTransferMessenger)
        {
            _apiClient = client;
            _transactionRepository = transactionRepository;
            _bankTransferMessenger = bankTransferMessenger;
        }
        public async Task<List<BankInfo>> GetBanks(ClientConfig config)
        {
            //Make request to the designated service provider
            var response = await _apiClient.Get<ApiResponse<List<BankDetail>>>(config?.GetAllBanksUrl!, config?.ProviderApiKey!);
            if (response.Data is null)
                throw new BadRequestException(response?.Message!);
            var result = response.Data
                .Select(s => new BankInfo { Code = s.Code, BankName = s.Name, LongName = s.Longcode })
                .ToList();

            return result;
            //return new List<BankInfo>() { Data = result, Message = response.Message, Status = response.Status };
        }

        public async Task<object> InitiateBankTransfer(ClientConfig config, BankTransferRequest query)
        {

            var transaction = new Transaction(Utils.GenerateTransactionReference(), query.Amount, Status.Pending);

            _transactionRepository.AddTransaction(transaction);

            await _transactionRepository.SaveChanges();

            var transferMessage = new FlutterwaveTransferMessage
            {
                Amount = query.Amount!.Value,
                TransactionId = transaction.Id,
                MaxRetry = query.MaxRetryAttempt,
                ProviderApikey = config.ProviderApiKey,
                TransferUrl = config.TransferUrl
            };

            await _bankTransferMessenger.Publish(transferMessage);

            return new { Message = "Your transfer is processing, We will let you know when its completed." };

            //return new ApiResponse<TransferResponse> { Message = "Your transfer is processing, We will let you know when its completed." };
        }

        public async Task<AccountInfo> ValidateAccountNumber(ClientConfig config, ValidateAccountNumberQuery query)
        {
            var data = new { account_number = query.AccountNumber, account_bank = query.Code };
            //Make request
            var response = await _apiClient.Post<ApiResponse<AccountDetail>>(data, config?.ValidateAccountNumberUrl!, config?.ProviderApiKey!);
            var listOfBanks = await GetBanks(config!);
            var bank = listOfBanks!.Where(x => x.Code == query.Code)
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
            return result;
            //return new ApiResponse<AccountInfo> { Data = result, Message = response!.Message, Status = response.Status };
        }

        public async Task<TransactionStatusResponse> StatusOfTransaction(ClientConfig config, string transactionReference)
        {
            var id = transactionReference;
            var response = await _apiClient.Get<ApiResponse<FlutterwaveTransferResponse>>($"{config.GetTransactionStatusUrl}/{id}", config!.ProviderApiKey!);
            if (response.Data is null)
                throw new BadRequestException(response.Message!);
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
            //{ Data = MapToTransactionStatus(response), Status = response.Status, Message = response.Message };
        }

        public async Task HandleBankTransfer(BankTransferMessage bankTransferMessage)
        {
            var transaction = await _transactionRepository.FindTransaction(bankTransferMessage.TransactionId);
            if (transaction is null)
                throw new BadRequestException($"Transaction with {bankTransferMessage.TransactionId} not found!");


            var data = new
            {
                account_bank = bankTransferMessage.BeneficiaryBankCode,
                account_number = bankTransferMessage.BeneficiaryAccountNumber,
                amount = transaction.Amount,
                narration = "",
                currency = "NGN",
                callback_url = "",
                reference = Utils.GenerateTransactionReference()
            };

            var response = await _apiClient.Post<ApiResponse<FlutterwaveTransferResponse>>(data, bankTransferMessage?.TransferUrl!, bankTransferMessage?.ProviderApikey!, true, bankTransferMessage!.MaxRetry);

            if (response.Data is null)
                throw new BadRequestException(response.Message!);
            if (response.Status == "true")
                transaction.TransferStatus = Status.Success;
            else
                transaction.TransferStatus = Status.Failed;
            _transactionRepository.UpdateTransaction(transaction);
            //call signalr- hub and pass the response, pass it 
            var flutterwaveResponse = MapToTransferResponse(response);

            await SignalRSender.SendDetailsThroughSignalR(flutterwaveResponse);

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
