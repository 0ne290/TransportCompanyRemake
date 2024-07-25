using Microsoft.Extensions.DependencyInjection;
using YooKassa.Entities;

namespace YooKassa.Extensions;

public static class ServiceCollection
{
    public static void AddTransientYooKassa(this IServiceCollection serviceCollection, string shopId, string secretKey, string paymentApiUrl)
    {
        serviceCollection.AddHttpClient(shopId, secretKey, paymentApiUrl);
        serviceCollection.AddTransient<PaymentService>();
    }
    
    public static void AddScopedYooKassa(this IServiceCollection serviceCollection, string shopId, string secretKey, string paymentApiUrl)
    {
        serviceCollection.AddHttpClient(shopId, secretKey, paymentApiUrl);
        serviceCollection.AddScoped<PaymentService>();
    }
    
    public static void AddSingletonYooKassa(this IServiceCollection serviceCollection, string shopId, string secretKey, string paymentApiUrl)
    {
        serviceCollection.AddHttpClient(shopId, secretKey, paymentApiUrl);
        serviceCollection.AddSingleton<PaymentService>();
    }
    
    private static void AddHttpClient(this IServiceCollection serviceCollection, string shopId, string secretKey, string paymentApiUrl)
    {
        serviceCollection.AddHttpClient("YooKassaPaymentApi", httpClient =>
        {
            httpClient.BaseAddress = new Uri(paymentApiUrl);
            httpClient.SetBasicAuthentication(shopId, secretKey);
        });
    }
}