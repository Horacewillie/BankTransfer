using BankTransfer.Domain.Exceptions;
using BankTransfer.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BankTransfer.Domain.CustomMiddleware
{
    public class GlobalExceptionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleException(ex, context);
            }
        }

        private static async Task HandleException(Exception ex, HttpContext context)
        {
            var response = context.Response;
            response.ContentType = "application/json";



            switch (ex)
            {
                case BadRequestException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }

            var errorResponse = new ErrorResponse()
            {
                Code = response.StatusCode,
                Description = GetErrorDescription(response.StatusCode),
                Message = ex.Message
            };
            var errorJson = JsonSerializer.Serialize(errorResponse);

            await context.Response!.WriteAsync(errorJson);
        }

        static string GetErrorDescription(int statuscode)
        {
            if (statuscode == 400)
            {
                return "Bad Request";
            }
            else if (statuscode == 404)
            {
                return "Not Found";
            }
            else
            {
                return "Internal Server Error";
            }
        }
    }
}
