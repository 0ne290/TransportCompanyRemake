using YooKassa.Entities.Payment;

namespace YooKassa.Entities;

public class PaymentService
{
    public PaymentService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("YooKassaPaymentApi");
    }

    public async Task<IssuedPayment> IssuePayment(UnissuedPayment unissuedPayment, string valueOfIdempotenceKey)
    {
        var unissuedPaymentJson = unissuedPayment.ToJson();
        Extensions.HttpClient.AddValueOfIdempotenceKeyHeader(_httpClient, valueOfIdempotenceKey);
        var issuedPaymentJson = await (await Extensions.HttpClient.PostJson(_httpClient, unissuedPaymentJson)).Content
            .ReadAsStringAsync();
        var issuedPayment = IssuedPayment.FromJson(issuedPaymentJson);

        return issuedPayment;
    }
    
    private readonly HttpClient _httpClient;
}