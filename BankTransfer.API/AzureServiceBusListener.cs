using Azure.Messaging.ServiceBus;
using BankTransfer.Core.Implementation;
using BankTransfer.Core.Interface;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;

namespace BankTransfer.API
{
    public class AzureServiceBusListener
    {
        public static ServiceProvider? ServiceProvider { get; internal set; }

        public static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            //(IProvider)_serviceProvider.GetService(typeof(FlutterwaveProvider)),
            try
            {
                if(args.Message.Subject == "paystack")
                {
                    var bankTransferMessage = Utils.Parse<PayStackTransferMessage>(args.Message.Body.ToArray());
                    var providerManager = (IProvider)ServiceProvider!.GetService(typeof(PaystackProvider))!;
                    await providerManager!.HandleBankTransfer(bankTransferMessage);
                }
                else
                {
                    var bankTransferMessage = Utils.Parse<FlutterwaveTransferMessage>(args.Message.Body.ToArray());
                    var providerManager = (IProvider)ServiceProvider!.GetService(typeof(FlutterwaveProvider))!;
                    await providerManager!.HandleBankTransfer(bankTransferMessage);
                }
                

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }


        public static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
