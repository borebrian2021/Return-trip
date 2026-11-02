using Microsoft.AspNetCore.Hosting;

namespace Return_trip.Services;

public sealed class SimulatedDepositPaymentService(IWebHostEnvironment environment) : IDepositPaymentService
{
    public Task<SimulatedDepositPaymentResult> SimulateSuccessfulPaymentAsync(
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        if (!environment.IsDevelopment())
        {
            throw new InvalidOperationException("Simulated payments are only available in the Development environment.");
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be greater than zero.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        return Task.FromResult(new SimulatedDepositPaymentResult(
            $"SIM-{Guid.NewGuid():N}",
            DateTime.UtcNow));
    }
}
