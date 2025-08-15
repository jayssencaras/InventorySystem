namespace Inventory.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = default!;
    public string Shortcode { get; set; } = default!;
    public Guid? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public List<Category> Children { get; set; } = new();
}
