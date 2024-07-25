using Domain.Entities;

namespace Domain.ServiceInterfaces;

public interface IOrderPaymentService
{
    public Task<string> IssuePayment(Order order);
}