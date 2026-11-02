namespace Return_trip.Services;

public interface IEmailSender
{
    Task SendAsync(
        string toAddress,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
