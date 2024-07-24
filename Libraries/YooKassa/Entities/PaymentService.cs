using YooKassa.Entities.Payment;

namespace YooKassa.Entities;

// Все-таки лучше сделать Confirmation одним фиксированным классом, если это возможно
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
