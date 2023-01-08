using BankTransfer.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace BankTransfer.Messaging.ClientTransferNotifier
{
    public class ClientNotifier
    {
        private readonly ApiClient _apiClient;

        public ClientNotifier(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task CallBack(ChannelMessage message, string apiKey)
        {
            try
            {
                var channelUrl = "";
                var response = await _apiClient.Post<string>(message, channelUrl, apiKey);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }
    }
}
