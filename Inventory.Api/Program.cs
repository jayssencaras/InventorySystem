using Inventory.Application.Abstractions;
using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.External;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Inventory.Api.Contracts;
using Inventory.Api.Validation;
using Inventory.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// DB connection
var conn = builder.Configuration.GetConnectionString("Default")
           ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
           ?? "Host=localhost;Port=5432;Database=inventory;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(conn));
builder.Services.AddSingleton<IWmsClient, WmsClient>();
builder.Services.AddSingleton<IAuditLogClient, AuditLogClient>();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.UseSwagger();
app.UseSwaggerUI();


(string userId, string userEmail) GetUser() => ("user-123", "user@example.com");

// Categories
app.MapPost("/categories", async (CreateCategoryRequest req, AppDbContext db) =>
{
    var (ok, errors) = req.Validate();
    if (!ok) return Results.ValidationProblem(errors);

    var entity = new Category
    {
        Name = req.Name.Trim(),
        Shortcode = req.Shortcode.Trim().ToUpperInvariant(),
        ParentCategoryId = req.ParentCategoryId
    };

    db.Categories.Add(entity);
    try
    {
        await db.SaveChangesAsync();
    }
    catch (DbUpdateException ex)
    {
        return Results.Conflict(new { message = "Category shortcode must be unique.", detail = ex.Message });
    }

    return Results.Created($"/categories/{entity.Id}", entity);
});

app.MapGet("/categories", async (AppDbContext db) =>
    Results.Ok(await db.Categories.AsNoTracking().ToListAsync()));

app.MapDelete("/categories/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var cat = await db.Categories.FindAsync(id);
    if (cat is null) return Results.NotFound();
    db.Categories.Remove(cat);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Suppliers
app.MapPost("/suppliers", async (CreateSupplierRequest req, AppDbContext db) =>
{
    var (ok, errors) = req.Validate();
    if (!ok) return Results.ValidationProblem(errors);

    var s = new Supplier
    {
        Name = req.Name.Trim(),
        Email = req.Email.Trim(),
        Currency = req.Currency.Trim().ToUpperInvariant(),
        Country = req.Country.Trim()
    };
    db.Suppliers.Add(s);
    await db.SaveChangesAsync();
    return Results.Created($"/suppliers/{s.Id}", s);
});

app.MapGet("/suppliers", async (AppDbContext db) =>
    Results.Ok(await db.Suppliers.AsNoTracking().ToListAsync()));


// Products
app.MapPost("/products", async (CreateProductRequest req, AppDbContext db, IWmsClient wms, IAuditLogClient audit) =>
{
    var (ok, errors) = req.Validate();
    if (!ok) return Results.ValidationProblem(errors);


    var supplierExists = await db.Suppliers.AnyAsync(s => s.Id == req.SupplierId);
    var category = await db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == req.CategoryId);
    if (!supplierExists) return Results.BadRequest(new { message = "Supplier does not exist." });
    if (category is null) return Results.BadRequest(new { message = "Category does not exist." });

    var p = new Product
    {
        SupplierId = req.SupplierId,
        CategoryId = req.CategoryId,
        Description = req.Description.Trim(),
        AcquisitionCostSupplierCurrency = req.AcquisitionCostSupplierCurrency,
        AcquisitionCostUSD = req.AcquisitionCostUSD,
        AcquireDate = req.AcquireDate
    };

    db.Products.Add(p);
    await db.SaveChangesAsync();

    // create product
    var wmsId = await wms.CreateProductAsync(
        new WmsProductCreateDto(
            p.Id.ToString(),
            p.Description,
            category.Shortcode,
            p.SupplierId.ToString()
        ),
        default);

    var user = GetUser();
    await audit.CreateLogAsync(new AuditLogEntry(user.userId, user.userEmail, "PRODUCT_CREATED", DateTime.UtcNow), default);

    return Results.Created($"/products/{p.Id}", new { p, wmsProductId = wmsId });
});

// Products - list 
app.MapGet("/products", async (AppDbContext db) =>
    Results.Ok(await db.Products.AsNoTracking().ToListAsync()));

app.MapGet("/products/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var p = await db.Products.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    return p is null ? Results.NotFound() : Results.Ok(p);
});

// Products - status change 
app.MapPost("/products/{id:guid}/status", async (Guid id, StatusChangeRequestDto req,
    AppDbContext db, IWmsClient wms, IEmailSender email, IAuditLogClient audit) =>
{
    var (ok, errors) = req.Validate();
    if (!ok) return Results.ValidationProblem(errors);

    var product = await db.Products.FindAsync(id);
    if (product is null) return Results.NotFound();

    try
    {
        await ProductStatusService.ChangeStatusAsync(product, req.NewStatus, req.Timestamp, db, wms, email);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }

    await db.SaveChangesAsync();

    var user = GetUser();
    await audit.CreateLogAsync(new AuditLogEntry(user.userId, user.userEmail, "PRODUCT_STATUS_CHANGED", DateTime.UtcNow), default);

    return Results.Ok(product);
});


app.Run();

record StatusChangeRequest(ProductStatus NewStatus, DateTime? Timestamp);
