namespace Inventory.Application.Abstractions;

public record AuditLogEntry(string userId, string email, string actionName, DateTime timestamp);

public interface IAuditLogClient
{
    Task CreateLogAsync(AuditLogEntry entry, CancellationToken ct);
}
