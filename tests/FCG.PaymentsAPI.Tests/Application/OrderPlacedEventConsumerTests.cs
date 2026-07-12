using FCG.Contracts.Events;
using FCG.PaymentsAPI.Application.Consumers;
using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace FCG.PaymentsAPI.Tests.Application;

public class OrderPlacedEventConsumerTests
{
    private readonly IPaymentRepository _paymentRepo = Substitute.For<IPaymentRepository>();
    private readonly ILogger<OrderPlacedEventConsumer> _logger =
        Substitute.For<ILogger<OrderPlacedEventConsumer>>();
    private readonly OrderPlacedEventConsumer _consumer;

    public OrderPlacedEventConsumerTests()
        => _consumer = new OrderPlacedEventConsumer(_paymentRepo, _logger);

    private static ConsumeContext<OrderPlacedEvent> CreateContext(OrderPlacedEvent evt)
    {
        var context = Substitute.For<ConsumeContext<OrderPlacedEvent>>();
        context.Message.Returns(evt);
        return context;
    }

    [Fact]
    public async Task Consume_ShouldProcessPaymentAndPublishEvent_WhenOrderIsNew()
    {
        var evt = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 79.90m);
        _paymentRepo.GetByOrderIdAsync(evt.OrderId).Returns((Payment?)null);
        var context = CreateContext(evt);

        await _consumer.Consume(context);

        await _paymentRepo.Received(1).AddAsync(Arg.Is<Payment>(p =>
            p.OrderId == evt.OrderId && p.Amount == evt.Amount));
        await _paymentRepo.Received(1).SaveChangesAsync();
        await context.Received(1).Publish(
            Arg.Is<PaymentProcessedEvent>(e => e.OrderId == evt.OrderId && e.Approved),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_ShouldSkipProcessing_WhenOrderAlreadyProcessed()
    {
        var evt = new OrderPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 79.90m);
        var existing = Payment.Create(evt.OrderId, evt.UserId, evt.GameId, evt.Amount);
        _paymentRepo.GetByOrderIdAsync(evt.OrderId).Returns(existing);
        var context = CreateContext(evt);

        await _consumer.Consume(context);

        await _paymentRepo.DidNotReceive().AddAsync(Arg.Any<Payment>());
        await context.DidNotReceive().Publish(
            Arg.Any<PaymentProcessedEvent>(), Arg.Any<CancellationToken>());
    }
}
