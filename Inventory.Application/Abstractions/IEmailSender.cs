namespace Inventory.Application.Abstractions;

public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string body, CancellationToken ct);
}
