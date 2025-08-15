using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid SupplierId { get; set; }
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = default!;
    public decimal AcquisitionCostSupplierCurrency { get; set; }
    public decimal AcquisitionCostUSD { get; set; }
    public DateTime? AcquireDate { get; set; }
    public DateTime? SoldDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public ProductStatus Status { get; set; } = ProductStatus.Created;
}
