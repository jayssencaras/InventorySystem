using Inventory.Application.Abstractions;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace Inventory.Infrastructure.Services;

public static class ProductStatusService
{
    public static async Task ChangeStatusAsync(
        Product product,
        ProductStatus newStatus,
        DateTime? timestamp,
        AppDbContext db,
        IWmsClient wms,
        IEmailSender email)
    {
        // Invalid: Cancelled/Returned -> Sold
        if ((product.Status is ProductStatus.Cancelled or ProductStatus.Returned) &&
            newStatus == ProductStatus.Sold)
        {
            throw new InvalidOperationException("Cancelled or returned products cannot be sold.");
        }

        product.Status = newStatus;
        var ts = timestamp ?? DateTime.UtcNow;

        switch (newStatus)
        {
            case ProductStatus.Sold:
                product.SoldDate = ts;

                var supplierEmail = await db.Suppliers
                    .Where(s => s.Id == product.SupplierId)
                    .Select(s => s.Email)
                    .FirstAsync();

                await email.SendAsync(supplierEmail, "Product Sold", $"Your product {product.Id} was sold.", default);
                await wms.DispatchProductAsync(product.Id.ToString(), default);
                break;

            case ProductStatus.Cancelled:
                product.CancelDate = ts;
                break;

            case ProductStatus.Returned:
                product.ReturnDate = ts;
                break;

            case ProductStatus.Created:
                product.AcquireDate = ts;
                break;
        }
    }
}
