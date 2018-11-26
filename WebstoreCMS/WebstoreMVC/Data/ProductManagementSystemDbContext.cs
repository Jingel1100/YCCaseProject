using Microsoft.EntityFrameworkCore;
using ProductsManagementSystemLibrary;
using WebstoreMVC.Models;

namespace WebstoreMVC
{
    public class ProductManagementSystemDbContext : DbContext
    {
        public ProductManagementSystemDbContext(DbContextOptions<ProductManagementSystemDbContext> options)
    : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<File> Files { get; set; }
    }
}