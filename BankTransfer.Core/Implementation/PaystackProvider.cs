using BankTransfer.Core.Interface;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Exceptions;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;
using BankTransfer.Infastructure.Repository;
using BankTransfer.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.Implementation
{
    public class PaystackProvider : IProvider
    {
        private readonly ApiClient _apiClient;
        private ITransactionRepository _transactionRepository;
        private readonly Messenger<PayStackTransferMessage> _bankTransferMessenger;

        public PaystackProvider(ApiClient client, 
            ITransactionRepository transactionRepository, Messenger<PayStackTransferMessage> bankTransferMessenger)
        {
            _apiClient = client;
            _transactionRepository = transactionRepository;
            _bankTransferMessenger = bankTransferMessenger;
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
            var receipientCode = await GetRecipientCode(config, query);
            if (receipientCode.Data is null)
                throw new BadRequestException(receipientCode.Message!);
            var receipient = receipientCode.Data.Recipient_Code;
            //check balance before proceeding --TO DO
            var transaction = new Transaction(Utils.GenerateTransactionReference(), query!.Amount!.Value, Status.Pending, receipient);

            _transactionRepository.AddTransaction(transaction);

            await _transactionRepository.SaveChanges();

            var transferMessage = new PayStackTransferMessage
            {
                Amount = query.Amount.Value,
                TransactionId = transaction.Id,
                MaxRetry = query.MaxRetryAttempt,
                ProviderApikey = config.ProviderApiKey,
                TransferUrl = config.TransferUrl
            };

            await _bankTransferMessenger.Publish(transferMessage);

            return new ApiResponse<TransferResponse> { Message = "Your transfer is processing, We will let you know when its completed." };


            //var data = new
            //{
            //    amount = query.Amount,
            //    recipient = receipientCode.Data.Recipient_Code,
            //    reference = Utils.GenerateTransactionReference(),
            //};
            //var response = await _apiClient.Post<ApiResponse<PaystackTransferResponse>>(data, config?.TransferUrl!, config?.ProviderApiKey!, true, query.MaxRetryAttempt);


            //if (response.Data is null)
            //    throw new BadRequestException(response.Message!);
            //return new ApiResponse<TransferResponse> { Data = MapToTransferResponse(response), Message = response.Message, Status = response.Status };
        }

        public async Task HandleBankTransfer(BankTransferMessage bankTransferMessage)
        {
            var transaction = await _transactionRepository.FindTransaction(bankTransferMessage.TransactionId);
            if (transaction is null) 
                throw new BadRequestException($"Transaction with {bankTransferMessage.TransactionId} not found!");

            var data = new
            {
                amount = transaction.Amount,
                recipient = transaction.Receipent,
                reference = transaction.TransactionReference,
            };

            var response = await _apiClient.Post<ApiResponse<PaystackTransferResponse>>(data, bankTransferMessage?.TransferUrl!, bankTransferMessage?.ProviderApikey!, true, bankTransferMessage.MaxRetry);

            if (response.Data is null)
                throw new BadRequestException(response.Message!);
            if(response.Status == "true")
                transaction.TransferStatus = Status.Success;
            else
                transaction.TransferStatus = Status.Failed;
            _transactionRepository.UpdateTransaction(transaction);

        }

        public async Task<ApiResponse<AccountInfo>> ValidateAccountNumber(ClientConfig config, ValidateAccountNumberQuery query)
        {
            var queryParameter = new Dictionary<string, string>
            {
                { "bank_code", query.Code.ToString() },
                { "account_number", query.AccountNumber.ToString() }
            };
            //Make request
            var response = await _apiClient.Get<ApiResponse<AccountDetail>>(config?.ValidateAccountNumberUrl!, config?.ProviderApiKey!, queryParameter);
            //Could Cache the getbanks api response, to prevent making call again --TO DO
            var listOfBanks = await GetBanks(config!);
            var bank = listOfBanks!.Data!.Where(x => x.Code == query.Code)
                .SingleOrDefault();
            if (response.Data is null)
                throw new BadRequestException(response.Message!);

            var result = new AccountInfo
            {
                AccountName = response?.Data.Account_Name,
                AccountNumber = response?.Data.Account_Number,
                BankName = bank?.BankName,
                BankCode = bank?.Code

            };
            return new ApiResponse<AccountInfo> { Data = result, Message = response!.Message, Status = response.Status };
        }

        public async Task<ApiResponse<TransactionStatusResponse>> StatusOfTransaction(ClientConfig config, string transactionReference)
        {
            var response = await _apiClient.Get<ApiResponse<PayStackVerificationResponse>>($"{config.GetTransactionStatusUrl}{transactionReference}", config!.ProviderApiKey!);
            if (response.Data is null)
                throw new BadRequestException(response.Message!);

            return new ApiResponse<TransactionStatusResponse> { Data = MapToTransactionStatus(response), Status = response.Status, Message = response.Message };
        }

        private async Task<ApiResponse<TransferRecepientInfo>> GetRecipientCode(ClientConfig config, BankTransferRequest query)
        {
            var data = new { bank_code = query.BeneficiaryBankCode, account_number = query.BeneficiaryAccountNumber };

            var transferReceipient = await _apiClient.Post<ApiResponse<TransferRecepientInfo>>
                (data, config?.GenerateReceipientUrl!, config?.ProviderApiKey!);
            return transferReceipient;
        }


        private static TransferResponse MapToTransferResponse(ApiResponse<PaystackTransferResponse> response)
        {
            var transferResponse = response?.Data;
            return new TransferResponse
            {
                Amount = transferResponse?.amount,
                TransactionReference = transferResponse?.reference,
                Status = transferResponse?.status,
                CurrencyCode = transferResponse?.currency,
                ResponseMessage = response!.Message,
                TransactionDateTime = transferResponse!.createdAt,
            };
        }

        private static TransactionStatusResponse MapToTransactionStatus(ApiResponse<PayStackVerificationResponse> response)
        {
            var transferResponse = response?.Data;

            return new TransactionStatusResponse
            {
                Amount = transferResponse?.amount,
                TransactionReference = transferResponse?.reference,
                CurrencyCode = transferResponse?.currency,
                BeneficiaryAccountName = transferResponse!.recipient.details.account_name,
                BeneficiaryAccountNumber = transferResponse.recipient.details.account_number,
                BeneficiaryBankCode = transferResponse.recipient.details.bank_code,
                TransactionDateTime = transferResponse.createdAt,
                ResponseMessage = response?.Message,
                Status = transferResponse?.status,
                SessionId = string.Empty
            };
        }
    }
}
