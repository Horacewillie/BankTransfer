using BankTransfer.Core.Implementation;
using BankTransfer.Core.SignalRService;
using BankTransfer.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankTransfer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankTransferServiceController : ControllerBase
    {
        private readonly ProviderManager _providerManager;
        private readonly ISignalRService _signalRService;
        public BankTransferServiceController(ProviderManager providerManager,ISignalRService signalRService)
        {
            _providerManager = providerManager;
            _signalRService = signalRService;
        }

        [HttpGet("/api/v1/core-banking/banks")]
        [Produces(typeof(ApiResponse<List<BankInfo>>))]
        public async Task<IActionResult> GetBanks([FromQuery] ProviderQuery providerQuery)
        {
            var listOfBanks = await _providerManager.GetAllBanks(providerQuery.Provider);
            return Ok(listOfBanks);
        }

        [HttpPost("/api/v1/core-banking/validateBankAccount")]
        [Produces(typeof(ApiResponse<AccountInfo>))]
        public async Task<IActionResult> ValidateAccountNumber([FromQuery] ValidateAccountNumberQuery validateAccountNumberQuery)
        {
            var accountInfo = await _providerManager.ValidateAccount(validateAccountNumberQuery);
            return Ok(accountInfo);
        }

        [HttpPost("/api/v1/core-banking/bankTransfer")]
        [Produces(typeof(ApiResponse<TransferResponse>))]
        public async Task<IActionResult> BankTransfer([FromBody] BankTransferRequest bankTransferRequest)
        {
            var transferResponse = await _providerManager.BankTransfer(bankTransferRequest);
            return Ok(transferResponse);
        }

        [HttpGet("/api/v1/core-banking/transaction/{transactionReference}")]
        [Produces(typeof(ApiResponse<TransferResponse>))]
        public async Task<IActionResult> GetTransactionStatus(string transactionReference, [FromQuery] string? provider)
        {
            var transferResponse = await _providerManager.GetTransactionStatus(transactionReference, provider);
            return Ok(transferResponse);
        }

        [HttpPost("/api/v1/core-banking/transaction/paymentCallbackUrl")]
        public async Task<IActionResult> FlutterwaveCallbackUrl([FromBody] ApiResponse<FlutterwaveTransferResponse> apiResponse)
        {
            await _providerManager.CallBackByFlutterWave(apiResponse);
            return Ok();
        }

        [HttpPost("/api/v1/core-banking/transaction/send")]
        //[Route("Send")]
        public async Task<IActionResult> SignalRHubTransferResponse(SignalRRequest signalRRequest)
        {
            Console.WriteLine($"Request got here {JsonConvert.SerializeObject(signalRRequest)}");
            var response = await _signalRService.SignalRHubTransfer(signalRRequest);

            return Ok(response);

        }


    }
}
