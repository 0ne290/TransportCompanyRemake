using Domain.Entities;

namespace Application.Interfaces;

public interface IOrderPaymentService
{
    public Task<string> GetPaymentUrl(Order order);
}