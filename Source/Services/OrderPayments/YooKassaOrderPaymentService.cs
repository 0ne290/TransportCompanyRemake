using Domain.ServiceInterfaces;
using YooKassa.Entities;

namespace OrderPayments;

public class YooKassaOrderPaymentService : IOrderPaymentService
{
    public YooKassaOrderPaymentService(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    private readonly PaymentService _paymentService;
}