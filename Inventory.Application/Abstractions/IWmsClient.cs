namespace Inventory.Application.Abstractions;

public record WmsProductCreateDto(string productId, string description, string? categoryShortcode, string supplierId);

public interface IWmsClient
{
    Task<string> CreateProductAsync(WmsProductCreateDto dto, CancellationToken ct);
    Task DispatchProductAsync(string productId, CancellationToken ct);
}
