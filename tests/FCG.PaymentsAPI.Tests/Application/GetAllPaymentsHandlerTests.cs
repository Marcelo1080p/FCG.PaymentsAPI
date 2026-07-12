using FCG.PaymentsAPI.Application.Payments.Queries.GetAllPayments;
using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Interfaces;
using NSubstitute;

namespace FCG.PaymentsAPI.Tests.Application;

public class GetAllPaymentsHandlerTests
{
    private readonly IPaymentRepository _paymentRepo = Substitute.For<IPaymentRepository>();
    private readonly GetAllPaymentsQueryHandler _handler;

    public GetAllPaymentsHandlerTests()
        => _handler = new GetAllPaymentsQueryHandler(_paymentRepo);

    [Fact]
    public async Task Handle_ShouldReturnAllPayments()
    {
        var payments = new List<Payment>
        {
            Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m),
            Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 20m)
        };
        _paymentRepo.GetAllAsync().Returns(payments);

        var result = await _handler.Handle(new GetAllPaymentsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoPaymentsExist()
    {
        _paymentRepo.GetAllAsync().Returns(new List<Payment>());

        var result = await _handler.Handle(new GetAllPaymentsQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}
