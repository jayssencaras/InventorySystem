using System.ComponentModel.DataAnnotations;

namespace Inventory.Api.Contracts;

// Categories
public class CreateCategoryRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(50)]
    public string Shortcode { get; set; } = default!;

    public Guid? ParentCategoryId { get; set; }
}

// Suppliers
public class CreateSupplierRequest
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, EmailAddress, MaxLength(320)]
    public string Email { get; set; } = default!;

    [Required, MaxLength(10)]
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO code (e.g., USD, SGD).")]
    public string Currency { get; set; } = default!;

    [Required, MaxLength(100)]
    public string Country { get; set; } = default!;
}

// Products
public class CreateProductRequest
{
    [Required]
    public Guid SupplierId { get; set; }

    [Required]
    public Guid CategoryId { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; } = default!;

    [Range(0, 1_000_000)]
    public decimal AcquisitionCostSupplierCurrency { get; set; }

    [Range(0, 1_000_000)]
    public decimal AcquisitionCostUSD { get; set; }

    public DateTime? AcquireDate { get; set; }
}

public class StatusChangeRequestDto
{
    [Required]
    public Inventory.Domain.Enums.ProductStatus NewStatus { get; set; }

    public DateTime? Timestamp { get; set; }
}
