using FCG.PaymentsAPI.Domain.Entities;
using MediatR;

namespace FCG.PaymentsAPI.Application.Payments.Queries.GetAllPayments;

public record GetAllPaymentsQuery : IRequest<IList<Payment>>;
