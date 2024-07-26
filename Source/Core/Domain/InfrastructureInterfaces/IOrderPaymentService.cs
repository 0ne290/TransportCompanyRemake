using Domain.Entities;

namespace Domain.InfrastructureInterfaces;

public interface IOrderPaymentService
{
    public Task<string> IssuePayment(Order order);
}