using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Enums;

namespace FCG.PaymentsAPI.Tests.Domain;

public class PaymentTests
{
    [Fact]
    public void Create_ShouldCreatePendingPayment_WhenDataIsValid()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var gameId = Guid.NewGuid();

        var payment = Payment.Create(orderId, userId, gameId, 59.90m);

        Assert.NotEqual(Guid.Empty, payment.Id);
        Assert.Equal(orderId, payment.OrderId);
        Assert.Equal(PaymentStatus.Pending, payment.Status);
        Assert.Null(payment.ProcessedAt);
    }

    [Fact]
    public void Create_ShouldThrow_WhenAmountIsNegative()
    {
        Assert.Throws<ArgumentException>(
            () => Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), -10m));
    }

    [Fact]
    public void Approve_ShouldSetStatusApproved_WhenPending()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m);

        payment.Approve();

        Assert.Equal(PaymentStatus.Approved, payment.Status);
        Assert.NotNull(payment.ProcessedAt);
    }

    [Fact]
    public void Approve_ShouldThrow_WhenAlreadyProcessed()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m);
        payment.Approve();

        Assert.Throws<InvalidOperationException>(() => payment.Approve());
    }

    [Fact]
    public void Reject_ShouldSetStatusRejected_WhenPending()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m);

        payment.Reject();

        Assert.Equal(PaymentStatus.Rejected, payment.Status);
        Assert.NotNull(payment.ProcessedAt);
    }

    [Fact]
    public void Reject_ShouldThrow_WhenAlreadyProcessed()
    {
        var payment = Payment.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 10m);
        payment.Reject();

        Assert.Throws<InvalidOperationException>(() => payment.Reject());
    }
}
