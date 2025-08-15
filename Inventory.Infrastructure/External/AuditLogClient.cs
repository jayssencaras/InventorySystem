using Inventory.Application.Abstractions;

namespace Inventory.Infrastructure.External;

public class AuditLogClient : IAuditLogClient
{
    public Task CreateLogAsync(AuditLogEntry entry, CancellationToken ct)
    {
        Console.WriteLine($"[AUDIT] {{ userId: {entry.userId}, email: {entry.email}, actionName: {entry.actionName}, timestamp: {entry.timestamp:o} }}");
        return Task.CompletedTask;
    }
}
