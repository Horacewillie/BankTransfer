using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.SignalRService
{
    public interface ISignalRService
    {
        Task<string> SignalRHubTransfer(SignalRRequest signalRRequest);
    }
}
