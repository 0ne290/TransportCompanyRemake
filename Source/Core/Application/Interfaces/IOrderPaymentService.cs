using Domain.Entities;

namespace Domain.Interfaces;

public interface IOrderPaymentService
{
    public Task<string> GetPaymentUrl(Order order);
}