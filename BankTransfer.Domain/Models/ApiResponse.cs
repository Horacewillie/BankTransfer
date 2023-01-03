using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
        public class ApiResponse<T> : BaseResponse
        {
            public T? Data { get; set; }
        }

        public class AccountDetail
        {
            public string? Account_Number { get; set; }
            public string? Account_Name { get; set; }
            public int Bank_Id { get; set; }
        }

        public class BankDetail
        {
            public string? Name { get; set; }
            public string? Slug { get; set; }
            public string? Code { get; set; }
            public string? Longcode { get; set; }
            public object? Gateway { get; set; }
            public bool Pay_with_bank { get; set; }
            public bool Active { get; set; }
            public bool Is_Deleted { get; set; }
            public string? Country { get; set; }
            public string? Currency { get; set; }
            public string? Type { get; set; }
            public int Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public class TransferRecepientInfo
        {
            public bool Active { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Currency { get; set; }
            public string Description { get; set; }
            public string Domain { get; set; }
            public string Email { get; set; }
            public int Id { get; set; }
            public int Integration { get; set; }
            public string Metadata { get; set; }
            public string Name { get; set; }
            public string Recipient_Code { get; set; }
            public string Type { get; set; }
            public DateTime UpdatedAt { get; set; }
            public bool Is_deleted { get; set; }
            public bool IsDeleted { get; set; }
            public RecepientInfo RecepientInfo { get; set; }
        }

        public class RecepientInfo
        {
            public string Authorization_Code { get; set; }
            public string Account_Number { get; set; }
            public string Account_Name { get; set; }
            public string Bank_Code { get; set; }
            public string Bank_Name { get; set; }
        }

        [Serializable]
        public class TransferResponse : BankTransferBase
        {

            public DateTime TransactionDateTime { get; set; }
            public string? ResponseMessage { get; set; }
            public int ResponseCode { get; set; }
            public string? SessionId { get; set; }
            public string? Status { get; set; }
        }

        public class TransactionStatusResponse : BankTransferBase
        {
            public DateTime TransactionDateTime { get; set; }
            public string? ResponseMessage { get; set; }
            public int ResponseCode { get; set; }
            public string? SessionId { get; set; }
            public string? Status { get; set; }
        }



        public class PaystackTransferResponse
        {
            public decimal amount { get; set; }
            public string currency { get; set; }
            public string reference { get; set; }
            public string status { get; set; }
            public DateTime createdAt { get; set; }
        }


        public class FlutterwaveTransferResponse
        {
            public string Account_Number { get; set; }
            public string Bank_Code { get; set; }
            public string Full_Name { get; set; }
            public DateTime Created_at { get; set; }
            public string Currency { get; set; }
            public int Amount { get; set; }
            public string Status { get; set; }
            public string Reference { get; set; }
            public int Is_Approved { get; set; }
        }

        public class PayStackVerificationResponse
        {
            public decimal amount { get; set; }
            public DateTime createdAt { get; set; }
            public string currency { get; set; }
            public string reference { get; set; }
            public string status { get; set; }
            public Recipient recipient { get; set; }

        }

        public class Details
        {
            public string account_number { get; set; }
            public string account_name { get; set; }
            public string bank_code { get; set; }
            public string bank_name { get; set; }
        }

        public class Recipient
        {
            public Details details { get; set; }
        }
}
