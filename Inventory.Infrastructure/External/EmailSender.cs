using Inventory.Application.Abstractions;

namespace Inventory.Infrastructure.External;

public class EmailSender : IEmailSender
{
    public Task SendAsync(string toEmail, string subject, string body, CancellationToken ct)
    {
        Console.WriteLine($"[EMAIL]\nTo: {toEmail}\nSubject: {subject}\nBody: {body}");
        return Task.CompletedTask;
    }
}
