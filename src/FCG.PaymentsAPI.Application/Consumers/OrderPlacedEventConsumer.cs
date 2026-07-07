using FCG.Contracts.Events;
using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FCG.PaymentsAPI.Application.Consumers;

public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ILogger<OrderPlacedEventConsumer> _logger;

    public OrderPlacedEventConsumer(
        IPaymentRepository paymentRepository,
        ILogger<OrderPlacedEventConsumer> logger)
    {
        _paymentRepository = paymentRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var evt = context.Message;

        _logger.LogInformation(
            "[PaymentsAPI] Pedido recebido — Order: {OrderId}, User: {UserId}, Valor: {Amount:C}",
            evt.OrderId, evt.UserId, evt.Amount);

        var existing = await _paymentRepository.GetByOrderIdAsync(evt.OrderId);
        if (existing is not null)
        {
            _logger.LogWarning("[PaymentsAPI] Pagamento já processado para o pedido {OrderId}.", evt.OrderId);
            return;
        }

        var payment = Payment.Create(evt.OrderId, evt.UserId, evt.GameId, evt.Amount);
        payment.Approve();

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        await context.Publish(new PaymentProcessedEvent(
            payment.Id, evt.OrderId, evt.UserId, evt.GameId, evt.Amount, true));

        _logger.LogInformation(
            "[PaymentsAPI] Pagamento aprovado — Payment: {PaymentId}, Order: {OrderId}",
            payment.Id, evt.OrderId);
    }
}
