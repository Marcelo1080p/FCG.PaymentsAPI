using FCG.PaymentsAPI.Domain.Entities;
using FCG.PaymentsAPI.Domain.Interfaces;
using MediatR;

namespace FCG.PaymentsAPI.Application.Payments.Queries.GetAllPayments;

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, IList<Payment>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetAllPaymentsQueryHandler(IPaymentRepository paymentRepository)
        => _paymentRepository = paymentRepository;

    public Task<IList<Payment>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
        => _paymentRepository.GetAllAsync();
}
