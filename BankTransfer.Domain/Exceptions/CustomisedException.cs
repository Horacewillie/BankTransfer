using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {

        }
        public BadRequestException(string message) : base(message)
        { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException()
        {

        }
        public NotFoundException(string message) : base(message)
        { }
    }
}
