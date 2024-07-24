using YooKassa.Entities.Payment;

namespace YooKassa.Entities;

public class YooKassa
{
    public YooKassa(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("YooKassaApi");
    }

    public IssuedPayment CreatePayment(UnissuedPayment unissuedPayment)
    {
        
    }
    
    private readonly HttpClient _httpClient;
}