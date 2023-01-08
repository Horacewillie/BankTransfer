using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Domain.Models
{
    public class ChannelMessage
    {
        public TransferResponse Data { get; set; }
        public DateTime TimeGenerated => DateTime.Now;
    }

    public class SignalRRequest 
    {
        public TransferResponse? Data { get; set; }
      
        public DateTime TimeGenerated => DateTime.Now;
    }

}
