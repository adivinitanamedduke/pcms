using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;
public class InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : DbContext(options)
{
    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }

}
