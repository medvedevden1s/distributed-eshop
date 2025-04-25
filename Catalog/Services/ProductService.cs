namespace Catalog.Services;

public class ProductService(ProductDbContext dbcontext)
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

    public async Task UpdateProductAsync(Product updateProduct, Product inputProduct)
    {
        updateProduct.Name = inputProduct.Name;
        updateProduct.Description = inputProduct.Description;
        updateProduct.Price = inputProduct.Price;
        
        dbcontext.Products.Update(updateProduct);
        await dbcontext.SaveChangesAsync();
    }

    public async Task DeleteProductAsync(Product deleteProduct)
    {
        dbcontext.Products.Remove(deleteProduct);
        await dbcontext.SaveChangesAsync();
    }
}