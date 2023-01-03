using BankTransfer.Core.Factory;
using BankTransfer.Core.Implementation;
using BankTransfer.Core.Interface;
using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.CustomMiddleware;
using BankTransfer.Domain.Exceptions;
using BankTransfer.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
// Add services to the container.
//services.Configure<IPaymentProviderConfig>(builder.Configuration.GetSection(nameof(PaymentProviderOptions.Config_Key)));
services.AddSingleton(p => configuration.GetSection(PaymentProviderOptions.Config_Key).Get<PaymentProviderOptions>());

services.AddScoped(p => configuration.GetSection(AppSettings.Config_Key).Get<AppSettings>());

//services.AddDistributedMemoryCache();

services.AddScoped<IProviderFactory, ProviderFactory>();

services.AddScoped<ProviderManager>();

services.AddScoped<PaystackProvider>()
    .AddScoped<IProvider, PaystackProvider>(s => s.GetService<PaystackProvider>()!);

services.AddScoped<FlutterwaveProvider>()
    .AddScoped<IProvider, FlutterwaveProvider>(s => s.GetService<FlutterwaveProvider>()!);

services.AddTransient<GlobalExceptionMiddleware>();

services.AddHttpClient<ApiClient>();

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
