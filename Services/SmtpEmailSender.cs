using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Return_trip.Services;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendAsync(
        string toAddress,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        var host = configuration["Email:Smtp:Host"];
        var portValue = configuration["Email:Smtp:Port"];
        var fromAddress = configuration["Email:Smtp:FromAddress"];

        if (string.IsNullOrWhiteSpace(host) ||
            !int.TryParse(portValue, out var port) ||
            port is < 1 or > 65535 ||
            string.IsNullOrWhiteSpace(fromAddress))
        {
            throw new InvalidOperationException(
                "SMTP is not configured. Set Email:Smtp:Host, Email:Smtp:Port, and Email:Smtp:FromAddress before sending email.");
        }

        var fromName = configuration["Email:Smtp:FromName"];
        var username = configuration["Email:Smtp:Username"];
        var password = configuration["Email:Smtp:Password"];
        var enableSsl = !bool.TryParse(configuration["Email:Smtp:EnableSsl"], out var sslConfigured) || sslConfigured;

        using var message = new MailMessage
        {
            From = string.IsNullOrWhiteSpace(fromName)
                ? new MailAddress(fromAddress)
                : new MailAddress(fromAddress, fromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true,
        };
        message.To.Add(new MailAddress(toAddress));

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            client.Credentials = new NetworkCredential(username, password);
        }

        await client.SendMailAsync(message, cancellationToken);
    }
}
