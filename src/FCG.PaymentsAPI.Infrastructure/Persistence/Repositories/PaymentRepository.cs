using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FCG.PaymentsAPI.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDbContext _context;

    public PaymentRepository(AppDbContext context) => _context = context;

    public Task<Payment?> GetByIdAsync(Guid id)
        => _context.Payments.FirstOrDefaultAsync(p => p.Id == id);

    public Task<Payment?> GetByOrderIdAsync(Guid orderId)
        => _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);

    public async Task<IList<Payment>> GetAllAsync()
        => await _context.Payments.ToListAsync();

    public async Task AddAsync(Payment payment)
        => await _context.Payments.AddAsync(payment);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}
