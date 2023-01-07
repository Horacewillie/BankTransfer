using Azure.Messaging.ServiceBus;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Messaging
{
    public class Messenger<T> where T : BankTransferMessage, new() //IMessenger
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusConfig _busConfig;
        readonly ServiceBusClientOptions clientOptions = new()
        {
            TransportType = ServiceBusTransportType.AmqpWebSockets
        };
        public Messenger(ServiceBusConfig busConfig)
        {
            _busConfig = busConfig;
            _client = new ServiceBusClient(_busConfig.BankTransferConnection, clientOptions);
        }

        //Generic, can publish any message
        public Task Publish(T message)
        {
            Task.Run(async () =>
            {
                var sender = _client.CreateSender("myqueue");
                try
                {
                    if(message is DefaultMessage defaultMessage)
                    {
                        Console.WriteLine("Just Default"); //TODO ---Take this out
                    }
                    else
                    {
                       
                        var data = Utils.ConvertToByte(message!);
                        ServiceBusMessage serviceBusMessage = new(data)
                        {
                            Subject = message.Label
                        };
                        await sender.SendMessageAsync(serviceBusMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Messaging Failed!!!");
                    Console.WriteLine(JsonConvert.SerializeObject(message));
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
                finally
                {
                    //Dispose since Di is not incharge of client and sender creation.
                    await sender.DisposeAsync();
                    await _client.DisposeAsync();
                }
            });
            return Task.CompletedTask;
            
        }
    }

    public class DefaultMessage
    {
    }


    //public interface IMessenger
    //{
    //    public Task Publish(T message);
    //}
}
