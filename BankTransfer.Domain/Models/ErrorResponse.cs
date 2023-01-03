using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class ErrorResponse
    {
        public ErrorResponse()
        {

        }
        public ErrorResponse(int code, string? description)
        {
            Code = code;
            Description = description;
        }

        public ErrorResponse(int code, string? description, string message)
            : this(code, description)
        {
            Message = message;
        }

        public int Code { get; set; }

        public string? Description { get; set; }

        public string? Message { get; set; }
    }
}
