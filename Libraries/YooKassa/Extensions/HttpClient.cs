using System.Net.Http.Headers;
using System.Text;

namespace YooKassa.Extensions;

public static class HttpClient
{
    public static void SetBasicAuthentication(this System.Net.Http.HttpClient httpClient, string login, string password) =>
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{password}")));

    public static void AddAcceptHeaderValue(this System.Net.Http.HttpClient httpClient, string value) =>
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(value));
    
    public static async Task<HttpResponseMessage> PostJson(this System.Net.Http.HttpClient httpClient, string json) =>
        await httpClient.PostAsync((string?)null, new StringContent(json, Encoding.UTF8, "application/json"));
}