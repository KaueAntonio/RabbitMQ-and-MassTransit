using Microsoft.EntityFrameworkCore;
using Shop.Infrastructure.Database.Models;

namespace Shop.Infrastructure.Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}
