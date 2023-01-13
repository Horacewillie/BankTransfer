namespace BankTransfer.Web
{
    public class TransferResponse
    {
        public DateTime TransactionDateTime { get; set; }
        public string? ResponseMessage { get; set; }
        public int ResponseCode { get; set; }
        public string? SessionId { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? BeneficiaryAccountNumber { get; set; }
        public string? BeneficiaryAccountName { get; set; }
        public string? BeneficiaryBankCode { get; set; }
        public string? TransactionReference { get; set; } //optional
        public string? CurrencyCode { get; set; }
    }
}
