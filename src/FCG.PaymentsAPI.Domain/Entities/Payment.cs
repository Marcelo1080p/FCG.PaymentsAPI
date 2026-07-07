using FCG.PaymentsAPI.Domain.Enums;

namespace FCG.PaymentsAPI.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid GameId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Payment() { }

    public static Payment Create(Guid orderId, Guid userId, Guid gameId, decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Valor do pagamento não pode ser negativo.");

        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            UserId = userId,
            GameId = gameId,
            Amount = amount,
            Status = PaymentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Pagamento já foi processado.");
        Status = PaymentStatus.Approved;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Reject()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Pagamento já foi processado.");
        Status = PaymentStatus.Rejected;
        ProcessedAt = DateTime.UtcNow;
    }
}
