using MassTransit;
using ServiceDefaults.Messaging.Events;

namespace Catalog.Services;

public class ProductService(ProductDbContext dbcontext, IBus bus)
{
    public async Task<IEnumerable<Product>> GetProductsAsync()
    {
        return await dbcontext.Products.ToListAsync();
    }
    
    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await dbcontext.Products.FindAsync(id);
    }

    public async Task CreateProductAsync(Product product)
    {
        dbcontext.Products.Add(product);
        await dbcontext.SaveChangesAsync();
    }

    public async Task UpdateProductAsync(Product updatedProduct, Product inputProduct)
    {
        // Check if the product price has changed
        if (updatedProduct.Price != inputProduct.Price)
        {
            // Create an integration event to notify other services of the price change
            var integrationEvent = new ProductPriceChangedEvent
            {
                // Use the database-generated product ID
                ProductId   = updatedProduct.Id,
                Name        = inputProduct.Name,
                Description = inputProduct.Description,
                Price       = inputProduct.Price,
                ImageUrl    = inputProduct.ImageUrl
            };

            // Publish the integration event to the message broker
            await bus.Publish(integrationEvent);
            
            /*
             * So if the database transaction fails after the publish operation, we could have stale or invalid messages in the system.
               In order to solve these dual write problem, we can use outbox pattern or Saga pattern to ensure these write operations are atomic.
               The outbox pattern solves this by storing the events in the same database transaction, then publishing after commit.
             */
        }

        
        updatedProduct.Name = inputProduct.Name;
        updatedProduct.Description = inputProduct.Description;
        updatedProduct.Price = inputProduct.Price;
        
        dbcontext.Products.Update(updatedProduct);
        await dbcontext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product deleteProduct)
    {
        dbcontext.Products.Remove(deleteProduct);
        await dbcontext.SaveChangesAsync();
    }
}