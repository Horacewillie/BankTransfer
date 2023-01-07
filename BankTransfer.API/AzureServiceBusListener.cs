using Azure.Messaging.ServiceBus;
using BankTransfer.Core.Implementation;
using BankTransfer.Domain.Helpers;
using BankTransfer.Domain.Models;

namespace BankTransfer.API
{
    public class AzureServiceBusListener
    {
        public static ServiceProvider? ServiceProvider { get; internal set; }

        public static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            try
            {
                var bankTransferMessage = Utils.Parse<BankTransferMessage>(args.Message.Body.ToArray());
                var providerManager = ServiceProvider!.GetService<ProviderManager>();
                await providerManager!.HandleBankTransfer(bankTransferMessage);

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
