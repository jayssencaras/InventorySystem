using Inventory.Application.Abstractions;

namespace Inventory.Infrastructure.External;

public class WmsClient : IWmsClient
{
    public Task<string> CreateProductAsync(WmsProductCreateDto dto, CancellationToken ct)
    {
        Console.WriteLine($"[WMS] createProduct => {{ productId: {dto.productId}, description: {dto.description}, categoryShortcode: {dto.categoryShortcode}, supplierId: {dto.supplierId} }}");
        return Task.FromResult(Guid.NewGuid().ToString()); // mock wmsProductId
    }

    public Task DispatchProductAsync(string productId, CancellationToken ct)
    {
        Console.WriteLine($"[WMS] dispatchProduct => productId={productId}");
        return Task.CompletedTask;
    }
}
