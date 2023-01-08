using BankTransfer.Domain.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Core.SignalRService
{
    public class SignalRService : ISignalRService
    {
        public async Task<string> SignalRHubTransfer(SignalRRequest signalRRequest)
        {
            Console.WriteLine("Heyy");
            HubConnection connection = null;
            try
            {
                var message = new BankTransferSignalRMessage()
                {
                    Data = signalRRequest.Data
                };
                string url = "https://localhost:7121/api/v1/core-banking/transaction/send";
                connection = new HubConnectionBuilder()
                    .WithUrl(url, option =>
                    {
                        option.Headers.Remove("Authorization");
                        option.Headers.Add("Authorization", "Bearer");
                    })
                    .WithAutomaticReconnect()
                    .Build();
                await connection.StartAsync();
                await connection.SendAsync("SendMessageToPostMan", message);
            }
            catch(Exception ex)
            {
                return $"Failure sending to client: {JsonConvert.SerializeObject(ex)}";
            }
            finally
            {
                connection?.DisposeAsync();
            }
            return $"Successfully sent to the client";
        }
    }
}
