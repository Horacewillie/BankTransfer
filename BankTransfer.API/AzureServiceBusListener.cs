using Azure.Messaging.ServiceBus;

namespace BankTransfer.API
{
    public class AzureServiceBusListener
    {
        public static ServiceProvider ServiceProvider { get; internal set; }

        public static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            Console.WriteLine($"Received: {body}");

            //complete the message.message is deleted from the queue. 
            await args.CompleteMessageAsync(args.Message);
        }


        public static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
