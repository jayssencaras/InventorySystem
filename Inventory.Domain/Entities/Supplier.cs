namespace Inventory.Domain.Entities;

public class Supplier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Currency { get; set; } = default!;
    public string Country { get; set; } = default!;
}
