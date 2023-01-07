﻿using Azure.Messaging.ServiceBus;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Messaging
{
    public class Messenger<T> where T : QueueMessage, new() //IMessenger
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
                try
                {
                    if(message is DefaultMessage defaultMessage)
                    {
                        Console.WriteLine("Hey yooo");
                    }
                    else
                    {
                        var sender = _client.CreateSender("bankTransfer");
                        var data = Utils.ConvertToByte(message!);
                        ServiceBusMessage serviceBusMessage = new(data);
                        await sender.SendMessageAsync(serviceBusMessage);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Messaging Failed!!!");
                    Console.WriteLine(JsonConvert.SerializeObject(message));
                    Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                }
            });
            return Task.CompletedTask;
            //var sender = _client.CreateSender("bankTransfer");
            //try
            //{
            //    var data = Utils.ConvertToByte(message!);
            //    ServiceBusMessage serviceBusMessage = new(data);
            //    //serviceBusMessage.ApplicationProperties["messageType"] = typeof(T).Name;
                
            //    await sender.SendMessageAsync(serviceBusMessage);
            //}
            //finally
            //{
                
            //    await sender.DisposeAsync();
            //    await _client.DisposeAsync();
            //}
        }
    }

    internal class DefaultMessage
    {
    }

    public abstract class QueueMessage
    {
        public abstract string QueueName { get; }
        public virtual string Label => "UpSkilling";
    }

    //public interface IMessenger
    //{
    //    public Task Publish(T message);
    //}
}