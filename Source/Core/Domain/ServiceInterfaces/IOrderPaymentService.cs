using Domain.Entities;

namespace Domain.ServiceInterfaces;

public interface IOrderPaymentService
{
    public string IssuePayment(Order order);
}