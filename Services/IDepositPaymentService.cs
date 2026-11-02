namespace Return_trip.Services;

public interface IDepositPaymentService
{
    Task<SimulatedDepositPaymentResult> SimulateSuccessfulPaymentAsync(
        decimal amount,
        CancellationToken cancellationToken = default);
}

public sealed record SimulatedDepositPaymentResult(string Reference, DateTime PaidAt);
