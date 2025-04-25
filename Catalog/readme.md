# Aspire Catalog Service

An ASP.NET Core Minimal API that provides CRUD operations for a product catalog backed by PostgreSQL. This service demonstrates a clean, layered architecture with the following components:

- **Domain Model** (`Product` entity)
- **Data Access** (`ProductDbContext` using EF Core)
- **Business Logic** (`ProductService`)
- **Presentation** (Minimal API endpoints in `ProductEndpoints`)

## Architecture Overview

This service follows a classic **N-Layer (Layered) Architecture** divided into four main layers:

![img.png](img.png)


1. **Domain Layer**
    - **What it stores:** Core business entities and rules.
    - **Example:** `Product` model in `Models/Product.cs` defines the domain data.
   ```csharp
   public class Product
   {
       public int Id { get; set; }
       public string Name { get; set; } = default!;
       public string Description { get; set; } = default!;
       public decimal Price { get; set; }
       public string ImageUrl { get; set; } = default!;
   }
   ```

2. **Data Access Layer**
    - **What it manages:** Database interactions and schema definitions.
    - **Example:** `ProductDbContext` in `Data/ProductDbContext.cs` uses EF Core to map `Product` to PostgreSQL.
   ```csharp
   public class ProductDbContext : DbContext
   {
       public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }
       public DbSet<Product> Products => Set<Product>();
   }
   ```

3. **Business Logic Layer**
    - **What it processes:** Core application rules, workflows, and data operations.
    - **Example:** `ProductService` in `Services/ProductService.cs` contains methods for CRUD operations against the `ProductDbContext`.
   ```csharp
   public class ProductService
   {
       private readonly ProductDbContext dbcontext;
       public ProductService(ProductDbContext dbcontext) { this.dbcontext = dbcontext; }

       public Task<IEnumerable<Product>> GetProductsAsync()  => dbcontext.Products.ToListAsync();
       public Task<Product?> GetProductByIdAsync(int id)      => dbcontext.Products.FindAsync(id).AsTask();
       public Task CreateProductAsync(Product p)             { dbcontext.Products.Add(p); return dbcontext.SaveChangesAsync(); }
       // UpdateProductAsync and DeleteProductAsync omitted for brevity
   }
   ```

4. **Presentation Layer**
    - **What it displays:** Exposes HTTP endpoints and handles user input/output.
    - **Example:** `ProductEndpoints` in `Endpoints/ProductEndpoints.cs` defines minimal API routes under `/products`.
   ```csharp
   app.MapGroup("/products")
      .MapGet("/", async (ProductService svc) => Results.Ok(await svc.GetProductsAsync()))
      .MapGet("/{id:int}", async (int id, ProductService svc) => { /* ... */ })
      .MapPost("/", async (Product p, ProductService svc) => { /* ... */ })
      // PUT, DELETE definitions...
   ```

**Data flow**:

```text
Client HTTP
    ↓
Presentation Layer (endpoints)
    ↓
Business Logic Layer (services)
    ↓
Data Access Layer (EF Core DbContext)
    ↓
Domain Layer (Product entities)
    ↓
PostgreSQL
```

---

## Project Structure

```text
Program.cs             Application startup, service registration, and endpoint mapping
Data/
  ProductDbContext.cs  EF Core DbContext for Products
Models/
  Product.cs           Domain entity definition
Services/
  ProductService.cs    Business logic and data operations for Products
Endpoints/
  ProductEndpoints.cs  Minimal API endpoint definitions under `/products`
```

## Dependency Injection

In `Program.cs`, services are registered as follows:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Register EF Core DbContext with Npgsql provider
builder.AddNpgsqlDbContext<ProductDbContext>(connectionName: "catalogdb");

// Register ProductService with scoped lifetime (one instance per request)
builder.Services.AddScoped<ProductService>();
```

## API Endpoints

The service exposes the following endpoints under the `/products` route:

| Method | Route              | Description               | Responses                   |
| ------ | ------------------ | ------------------------- | --------------------------- |
| GET    | `/products`        | List all products         | 200 OK (`List<Product>`)    |
| GET    | `/products/{id}`   | Get product by ID         | 200 OK (`Product`) / 404    |
| POST   | `/products`        | Create a new product      | 201 Created (`Product`)     |
| PUT    | `/products/{id}`   | Update existing product   | 204 No Content / 404        |
| DELETE | `/products/{id}`   | Delete product by ID      | 204 No Content / 404        |

### Example Usage

```http
GET https://localhost:5001/products
Accept: application/json
```
```http
GET https://localhost:5001/products/1
Accept: application/json
```
```http
POST https://localhost:5001/products
Content-Type: application/json

{
  "name": "Solar Powered Flashlight",
  "description": "A great tool for outdoor adventures",
  "price": 19.99,
  "imageUrl": "product1.png"
}
```
```http
PUT https://localhost:5001/products/1
Content-Type: application/json

{
  "name": "Updated Flashlight",
  "description": "Brighter and longer-lasting",
  "price": 24.99,
  "imageUrl": "product1-new.png"
}
```
```http
DELETE https://localhost:5001/products/1
```
