using BankTransfer.Domain.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankTransfer.Messaging
{
    public class ApiResponseHandler
    {
        public async Task<T> Handle<T>(HttpResponseMessage httpResponseMessage)
           where T : class
        {
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();

                if (jsonData is T)
                {
                    return jsonData as T;
                }
                var data = JsonConvert.DeserializeObject<T>(jsonData);
                return data!;
            }
            else
            {
                var jsonData = await httpResponseMessage.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<T>(jsonData);
                return data!;
            }
        }

        public static async Task<Envelope<string>> Handle(HttpResponseMessage response, bool acceptEmptyResponse = true)
        {
            var responseJson = await response!.Content.ReadAsStringAsync();

            return Envelope<string>.Ok(responseJson);
        }

    }
}

