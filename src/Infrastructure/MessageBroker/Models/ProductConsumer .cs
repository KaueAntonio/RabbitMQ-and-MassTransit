using MassTransit;
using Shop.Infrastructure.Database;
using Shop.Infrastructure.Database.Models;

namespace Shop.Infrastructure.MessageBroker.Models;

public class ProductConsumer(ApplicationDbContext dbContext) : IConsumer<Product>
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task Consume(ConsumeContext<Product> context)
    {
        var product = context.Message;

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        Console.WriteLine($"Product {product.Name} added.");
    }
}
