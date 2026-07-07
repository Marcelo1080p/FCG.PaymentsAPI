using FCG.PaymentsAPI.Domain.Entities;

namespace FCG.PaymentsAPI.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<Payment?> GetByOrderIdAsync(Guid orderId);
    Task<IList<Payment>> GetAllAsync();
    Task AddAsync(Payment payment);
    Task SaveChangesAsync();
}
