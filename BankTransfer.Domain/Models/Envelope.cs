using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class Envelope<T>
    {
        public T? Data { get; set; }

        //public string? Message { get; set; }

        //[JsonIgnore]
        //public ResultStatus RequestStatus { get; set; }
        //public string? Status => RequestStatus.ToString();


        public static Envelope<T> Ok(T data)
        {
            return new Envelope<T>()
            {
                Data = data,
                //RequestStatus = ResultStatus.True,
            };
        }

        public static Envelope<T> Error(T data)
        {
            return new Envelope<T>()
            {
                Data = data,
                //RequestStatus = ResultStatus.True,
            };
        }

        //public Envelope<TResult> Cast<TResult>(TResult data)
        //{
        //    return new Envelope<TResult>()
        //    {
        //        Data = data,
        //        RequestStatus = RequestStatus
        //    };
        //}
    }

    public static class Envelope
    {
        static Envelope()
        {

        }
        public static string Code { get; set; }
        public static string Description { get; set; }

        public static string Message { get; set; }

        public static Envelope<TData> Ok<TData>(TData data)
        {
            return new Envelope<TData>
            {
                Data = data,
                //RequestStatus = ResultStatus.True,
            };
        }
    }
}
