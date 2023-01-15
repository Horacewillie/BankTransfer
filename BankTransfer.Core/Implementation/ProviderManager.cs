using BankTransfer.Core.Factory;
using BankTransfer.Core.Interface;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Exceptions;
using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.Implementation
{
    public class ProviderManager
    {
        private readonly IProviderFactory _providerFactory;
        private readonly PaymentProviderOptions _paymentProviderOptions;


        public ProviderManager(IProviderFactory providerFactory, PaymentProviderOptions paymentProviderOptions)
        {
            _providerFactory = providerFactory;
            _paymentProviderOptions = paymentProviderOptions;
        }

        public async Task<List<BankInfo>> GetAllBanks(string? provider)
        {
            var (config, paymentProvider) = GetProviderToUse(provider);
            var response = await paymentProvider.GetBanks(config);
            return response;
        }

        public async Task<AccountInfo> ValidateAccount(ValidateAccountNumberQuery query)
        {
            ValidateValidationQuery(query);

            var (config, paymentProvider) = GetProviderToUse(query.Provider);
            var response = await paymentProvider.ValidateAccountNumber(config, query);
            return response;

        }

        public async Task<object> BankTransfer(BankTransferRequest bankTransferRequest)
        {
            ValidateBankRequest(bankTransferRequest);
            var (config, paymentProvider) = GetProviderToUse(bankTransferRequest.Provider);
            var response = await paymentProvider.InitiateBankTransfer(config, bankTransferRequest);
            return response;
        }

        public async Task<TransactionStatusResponse> GetTransactionStatus(string transactionReference, string? provider)
        {
            if (string.IsNullOrEmpty(transactionReference)) throw new BadRequestException("Transaction Reference must be supplied.");
            var (config, paymentProvider) = GetProviderToUse(provider);
            var response = await paymentProvider.StatusOfTransaction(config, transactionReference);
            return response;
        }

        public async Task CallBackByFlutterWave(ApiResponse<FlutterwaveTransferResponse> response)
        {
            if (response.Data!.Reference is null) throw new BadRequestException("No reference for this transaction");
            //Persist transaction or push notification of transaction status to client.
        }

        private (ClientConfig configToUse, IProvider providerToUse) GetProviderToUse(string? provider)
        {
            var selectedProviderOptions = GetPaymentProviderDetails(provider);

            if (selectedProviderOptions!.Name!.Equals("flutterwave", StringComparison.OrdinalIgnoreCase))
            {
                return (selectedProviderOptions, _providerFactory.GetProvider(ProviderEnum.Flutterwave));
            }
            return (selectedProviderOptions, _providerFactory.GetProvider(ProviderEnum.PayStack));
        }

        private ClientConfig? GetPaymentProviderDetails(string? provider)
        {
            var paymentProvider = _paymentProviderOptions.GetPaymentProvider(provider);

            var selectedProviderOptions = _paymentProviderOptions.PaymentProviderConfigs?.FirstOrDefault(x => x.Name!.Equals(paymentProvider, StringComparison.OrdinalIgnoreCase));
            return selectedProviderOptions;
        }

        private static void ValidateBankRequest(BankTransferRequest bankTransferRequest)
        {
            if (string.IsNullOrEmpty(bankTransferRequest.BeneficiaryBankCode)) throw new BadRequestException("Account code is required");
            if (string.IsNullOrEmpty(bankTransferRequest.BeneficiaryAccountNumber)) throw new BadRequestException("Account Number is required");
            if (bankTransferRequest.Amount is null) throw new BadRequestException("Amount is required");
        }

        private static void ValidateValidationQuery(ValidateAccountNumberQuery query)
        {
            if (string.IsNullOrEmpty(query.AccountNumber))
                throw new BadRequestException("Account Number is required");
            if (string.IsNullOrEmpty(query.Code))
                throw new BadRequestException("Bank Code is required");
        }


    }
}
