using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace BankTransfer.Web.Hubs
{
    public class BankTransferHub : Hub
    {
        public async Task SendMessage(TransferResponse transferResponse)
        {
            Console.WriteLine("Hey Manny");
            var valueToSend = JsonSerializer.Serialize(transferResponse);
            Console.WriteLine(valueToSend);
            await Clients.All.SendAsync("ReceiveMessage", valueToSend);
        }
    }
}
