using Azure.Messaging.ServiceBus;
using BankTransfer.API;
using BankTransfer.Core.Factory;
using BankTransfer.Core.Implementation;
using BankTransfer.Core.Interface;
using BankTransfer.Core.SignalRService;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.CustomMiddleware;
using BankTransfer.Domain.Exceptions;
using BankTransfer.Infastructure;
using BankTransfer.Infastructure.Repository;
using BankTransfer.Messaging;
using BankTransfer.Messaging.ClientTransferNotifier;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
// Add services to the container.
//services.Configure<IPaymentProviderConfig>(builder.Configuration.GetSection(nameof(PaymentProviderOptions.Config_Key)));
services.AddSingleton(p => configuration.GetSection(PaymentProviderOptions.Config_Key)
.Get<PaymentProviderOptions>());

services.AddScoped(p => configuration.GetSection(AppSettings.Config_Key)
.Get<AppSettings>());

services.AddSingleton(p => GetBusConfig());

services.AddSingleton(typeof(Messenger<>));

services.AddDbContext<BankTransferDbContext>(options =>
{
    options.UseSqlServer(GetDbConnection())
    .EnableSensitiveDataLogging();
});

services.AddScoped<ClientNotifier>();
string GetDbConnection() => configuration.GetSection(AppSettings.Config_Key)
    .Get<AppSettings>().DbConnectionString!;

//services.AddSingleton<IMessenger, Messenger>();

//services.AddSingleton((serviceProvider) =>
//{
//    ServiceBusConfig options = serviceProvider.GetService<ServiceBusConfig>()!;

//    return new ServiceBusClient(options.BankTransferConnection);
//});

ServiceBusConfig GetBusConfig()
{
    var busConfig = configuration.GetSection(ServiceBusConfig.Config_Key)
        .Get<ServiceBusConfig>();
    return busConfig;
}

services.AddScoped<ISignalRService, SignalRService>();

services.AddScoped<IProviderFactory, ProviderFactory>();

services.AddScoped<ProviderManager>();

services.AddScoped<PaystackProvider>()
    .AddScoped<IProvider, PaystackProvider>(s => s.GetService<PaystackProvider>()!);

services.AddScoped<FlutterwaveProvider>()
    .AddScoped<IProvider, FlutterwaveProvider>(s => s.GetService<FlutterwaveProvider>()!);

services.AddTransient<GlobalExceptionMiddleware>();

services.AddHttpClient<ApiClient>();

services.AddScoped<ITransactionRepository, TransactionRepository>();

await ConfigureBusServices();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = HandleModelStateError;
    });
IActionResult HandleModelStateError(ActionContext arg)
{
    var allErrors = arg.ModelState.Values.SelectMany(v => v.Errors.Select(b => b.ErrorMessage + "  "));
    string errorMessage = string.Concat(allErrors);
    return new BadRequestObjectResult(new BadRequestException(errorMessage));
}
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

async Task ConfigureBusServices()
{
    ServiceCollection services = new();

    services!.AddSingleton(p => configuration!.GetSection(PaymentProviderOptions.Config_Key).Get<PaymentProviderOptions>());

    services!.AddScoped(p => configuration!.GetSection(AppSettings.Config_Key)
    .Get<AppSettings>());

    services!.AddSingleton(p => GetBusConfig());

    services.AddSingleton(typeof(Messenger<>));

    services.AddDbContext<BankTransferDbContext>(options =>
    {
        options.UseSqlServer(GetDbConnection())
        .EnableSensitiveDataLogging();
    }, ServiceLifetime.Transient);

    services.AddScoped<ClientNotifier>();

    services.AddSignalR();

    services.AddTransient<IProviderFactory, ProviderFactory>();

    services.AddTransient<ProviderManager>();

    services.AddTransient<PaystackProvider>()
        .AddScoped<IProvider, PaystackProvider>(s => s.GetService<PaystackProvider>()!);

    services.AddTransient<FlutterwaveProvider>()
        .AddScoped<IProvider, FlutterwaveProvider>(s => s.GetService<FlutterwaveProvider>()!);

    services.AddHttpClient<ApiClient>();

    services.AddScoped<ITransactionRepository, TransactionRepository>();

    var busConfig = GetBusConfig();

    AzureServiceBusListener.ServiceProvider = services.BuildServiceProvider();

    await ServiceBusRegistry.RegisterListeners(busConfig);
}

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ApiKeyAuthenticationMiddleware>();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
