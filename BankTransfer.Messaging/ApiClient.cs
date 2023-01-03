using BankTransfer.Domain.Models;
using Newtonsoft.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;
using System.Net;

namespace BankTransfer.Messaging
{
    public class ApiClient
    {
        readonly HttpClient _httpClient;
        readonly ApiResponseHandler? _handler = new ApiResponseHandler();
        //ExponentialRetry with Polly
        private readonly IAsyncPolicy<HttpResponseMessage> _retryPolicy =
           Policy<HttpResponseMessage>
               .Handle<HttpRequestException>()
               .OrResult(c => c.StatusCode >= HttpStatusCode.InternalServerError || HttpStatusCode.RequestTimeout == c.StatusCode || c.StatusCode == HttpStatusCode.BadRequest)
               .WaitAndRetryAsync(Backoff.ExponentialBackoff(TimeSpan.FromSeconds(1), 5));

        public ApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        static void PrepRequest(HttpContent? content = null)
        {
            if (content is not null)
            {

                content.Headers.Remove("Content-Type");
                content.Headers.Add("Content-Type", "application/json");
            }
        }

        public async Task<T> Post<T>(object data, string url, string apiKey, bool retry = false, int maxRetryAttempt = 0)
            where T : class
        {

            var content = new StringContent(JsonConvert.SerializeObject(data));
            PrepRequest(content);
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{apiKey}");
            if (retry && maxRetryAttempt > 0)
            {
                var retryResponse = await _retryPolicy.ExecuteAsync(async () => await _httpClient.PostAsync(url, content));
                if (retryResponse.IsSuccessStatusCode)
                {
                    var jsonData = await retryResponse.Content.ReadAsStringAsync();


                    if (jsonData is T)
                    {
                        return jsonData as T;
                    }
                    var result = JsonConvert.DeserializeObject<T>(jsonData);
                    return result!;
                }
                else
                {
                    int retries = 0;
                    int backoffInterval = 1000;
                    while (retries < maxRetryAttempt)
                    {
                        backoffInterval *= 2;

                        // Delay for a few minutes
                        await Task.Delay(backoffInterval);

                        //Make the request again
                        var failureRetrial = await _retryPolicy.ExecuteAsync(async () => await _httpClient.PostAsync(url, content));
                        var retryResponseContent = await failureRetrial.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(retryResponseContent);
                        if (failureRetrial.IsSuccessStatusCode)
                            return result!;
                        retries++;
                    }
                }
            }
            var response = await _httpClient.PostAsync(url, content);
            return await _handler!.Handle<T>(response);

        }

        public async Task<T> Get<T>(string url, string providerApiKey, Dictionary<string, string> data = null)
            where T : class
        {

            string queryString = "?";
            PrepRequest();
            if (data?.Any() == true)
            {
                foreach (var pair in data)
                {
                    queryString += $"{pair.Key}={pair.Value}&";
                }
                url += queryString;
            }
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue($"Bearer", $"{providerApiKey}");
            var response = await _httpClient.GetAsync(url);
            return await _handler!.Handle<T>(response);

        }

        public async Task<Envelope<string>> Get(string url)
        {
            PrepRequest();
            var response = await _httpClient.GetAsync(url);
            return await ApiResponseHandler.Handle(response);
        }


    }
}