using BankTransfer.Domain.Models;
//using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Messaging.SignalRClient
{
    public class SignalRSender
    {
        public static async Task SendDetailsThroughSignalR(TransferResponse detailsToBeSent)
        {
            Console.WriteLine(detailsToBeSent);
            Console.WriteLine("Hello World!");

            HubConnection connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7015/BankTransferHub")
                .WithAutomaticReconnect()
                .Build();

            await connection.StartAsync();

            await connection.InvokeAsync("SendMessage", detailsToBeSent);
        }
    }
}
