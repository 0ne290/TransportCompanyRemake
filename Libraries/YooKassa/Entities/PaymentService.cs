using YooKassa.Entities.Payment;

namespace YooKassa.Entities;

public class PaymentService
{
    public PaymentService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("YooKassaPaymentApi");
    }

    public IssuedPayment CreatePayment(UnissuedPayment unissuedPayment)
    {
        
    }
    
    private readonly HttpClient _httpClient;
}