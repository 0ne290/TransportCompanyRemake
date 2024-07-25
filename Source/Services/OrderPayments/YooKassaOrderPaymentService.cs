using System.Globalization;
using Domain.Entities;
using Domain.ServiceInterfaces;
using YooKassa.Constants;
using YooKassa.Entities;
using YooKassa.Entities.Payment;

namespace OrderPayments;

public class YooKassaOrderPaymentService : IOrderPaymentService
{
    public YooKassaOrderPaymentService(PaymentService paymentService, string returnUrl)
    {
        _paymentService = paymentService;
        _returnUrl = returnUrl;
    }
    
    public async Task<string> IssuePayment(Order order)
    {
        var unissuedPayment = new UnissuedPayment
        {
            Amount = new Amount
            {
                Value = order.Price.ToString(CultureInfo.InvariantCulture),
                Currency = Currencies.Rub
            },
            Confirmation = new Confirmation
            {
                Type = ConfirmationTypes.Redirect,
                ReturnUrl = _returnUrl
            },
            Description = $"Оплата заказа {order.Guid}."
        };
        
        var issuedPayment = await _paymentService.IssuePayment(unissuedPayment);

        return issuedPayment.Confirmation.ConfirmationUrl;
    }

    private readonly PaymentService _paymentService;

    private readonly string _returnUrl;
}