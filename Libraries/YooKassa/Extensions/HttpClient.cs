using System.Net.Http.Headers;
using System.Text;

namespace YooKassa.Extensions;

public static class HttpClient
{
    public static void SetBasicAuthentication(this System.Net.Http.HttpClient httpClient, string login, string password) =>
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{password}")));
}