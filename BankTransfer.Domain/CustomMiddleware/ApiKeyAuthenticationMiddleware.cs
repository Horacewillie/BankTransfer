using BankTransfer.Domain.Configuration;
using BankTransfer.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace BankTransfer.Domain.CustomMiddleware
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        private const string apiKey = "ApiKey";
        public ApiKeyAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            var errorResponse = new ErrorResponse();

            if (!context.Request.Headers.TryGetValue(apiKey, out var extractedValue))
            {
                await UnAuthorizedResponse(context, errorResponse);
                return;
            }
            var appSettings = context.RequestServices.GetRequiredService<AppSettings>();
            var key = appSettings.ApiKey;
            if (!key!.Equals(extractedValue))
            {
                await UnAuthorizedResponse(context, errorResponse);
                return;
            }

            await _next(context);
        }

        private static async Task UnAuthorizedResponse(HttpContext context, ErrorResponse errorResponse)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            errorResponse.Code = context.Response.StatusCode;
            errorResponse.Message = "No Authorization Header was found";
            errorResponse.Description = "UnAuthorized";
            var errorJson = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(errorJson);
        }
    }
}
