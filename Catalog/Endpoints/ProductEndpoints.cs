namespace Catalog.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/products");

        // GET all
        group.MapGet("/", async (ProductService service) =>
        {
            var products = await service.GetProductsAsync();
            return Results.Ok(products);
        })
        .WithName("GetAllProducts")
        .Produces<List<Product>>(StatusCodes.Status200OK);

        // GET by ID
        group.MapGet("/{id:int}", async (int id, ProductService service) =>
        {
            var product = await service.GetProductByIdAsync(id);
            return product is null ? Results.NotFound() : Results.Ok(product);
        })
        .WithName("GetProductById")
        .Produces<Product>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        // POST (Create)
        group.MapPost("/", async (Product product, ProductService service) =>
        {
            await service.CreateProductAsync(product);
            return Results.Created($"/products/{product.Id}", product);
        })
        .WithName("CreateProduct")
        .Produces<Product>(StatusCodes.Status201Created);

        // PUT (Update)
        group.MapPut("/{id}", async (int id, Product inputProduct, ProductService service) =>
        {
            var existing = await service.GetProductByIdAsync(id);
            if (existing is null)
                return Results.NotFound();

            await service.UpdateProductAsync(existing, inputProduct);
            return Results.NoContent();
        })
        .WithName("UpdateProduct")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);

        // DELETE
        group.MapDelete("/{id:int}", async (int id, ProductService service) =>
        {
            var deletedProduct = await service.GetProductByIdAsync(id);
            if (deletedProduct is null)
                return Results.NotFound();

            await service.DeleteProductAsync(deletedProduct);
            return Results.NoContent();
        })
        .WithName("DeleteProduct")
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status204NoContent);
    }
}
