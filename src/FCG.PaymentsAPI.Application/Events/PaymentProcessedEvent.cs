namespace FCG.Contracts.Events;

public record PaymentProcessedEvent(Guid PaymentId, Guid OrderId, Guid UserId, Guid GameId, decimal Amount, bool Approved);
