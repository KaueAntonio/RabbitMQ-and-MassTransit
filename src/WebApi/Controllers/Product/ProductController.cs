using MassTransit;
using Shop.Infrastructure.Database.Models;

namespace Shop.Api.Controllers;

public static class ProductController
{
    private static readonly List<Product> Products =
    [
        new Product { Id = 1, Name = "Product 1", Price = 10 },
        new Product { Id = 2, Name = "Product 2", Price = 20 }
    ];

    public static void MapRoutes(IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/products");

        group.MapGet("/", GetAllProducts);
        group.MapGet("/{id:int}", GetProductById);
        group.MapPost("/", AddProduct);
        group.MapPut("/{id:int}", UpdateProduct);
        group.MapDelete("/{id:int}", DeleteProduct);
    }

    private static IResult GetAllProducts()
    {
        return Results.Ok(Products);
    }

    private static IResult GetProductById(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(product);
    }

    private static IResult AddProduct(Product product, IBus bus)
    {
        product.Id = Products.Max(p => p.Id) + 1;
        Products.Add(product);

        bus.Publish(product);

        return Results.Created($"/products/{product.Id}", product);
    }

    private static IResult UpdateProduct(int id, Product updatedProduct)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return Results.NotFound();
        }

        product.Name = updatedProduct.Name;
        product.Price = updatedProduct.Price;

        return Results.NoContent();
    }

    private static IResult DeleteProduct(int id)
    {
        var product = Products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return Results.NotFound();
        }

        Products.Remove(product);
        return Results.NoContent();
    }
}
