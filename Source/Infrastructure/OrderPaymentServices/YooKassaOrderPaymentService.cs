using System.Globalization;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using YooKassa.Constants;
using YooKassa.Entities;
using YooKassa.Entities.Payment;

namespace OrderPaymentServices;

public class YooKassaOrderPaymentService : IOrderPaymentService
{
    public YooKassaOrderPaymentService(PaymentService paymentService, string returnUrl)
    {
        _paymentService = paymentService;
        _returnUrl = returnUrl;
    }
    
    public async Task<string> GetPaymentUrl(Order order)
    {
        if (order.Status != OrderStatuses.PerformersAssigned)
            throw new ArgumentException("Order.Status is invalid", nameof(order));
        
        var unissuedPayment = new UnissuedPayment
        {
            Amount = new Amount
            {
                Value = order.Price!.Value.ToString(CultureInfo.InvariantCulture),
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