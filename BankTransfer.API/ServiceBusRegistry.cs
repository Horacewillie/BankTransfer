using Azure.Messaging.ServiceBus;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Models;
using System.Diagnostics;

namespace BankTransfer.API
{
    public static class ServiceBusRegistry
    {
        public static async Task RegisterListeners(ServiceBusConfig busConfig)
        {
            var clientOptions = new ServiceBusClientOptions()
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets
            };

            ServiceBusClient client = new(busConfig.BankTransferConnection, clientOptions);

            ServiceBusProcessor processor = client.CreateProcessor(QueueConfigs.Transfer, new ServiceBusProcessorOptions());

            processor.ProcessMessageAsync += AzureServiceBusListener.MessageHandler;

            // add handler to process any errors
            processor.ProcessErrorAsync += AzureServiceBusListener.ErrorHandler;

            // start processing
            await processor.StartProcessingAsync();
        }
    }
}
