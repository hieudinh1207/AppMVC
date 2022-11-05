
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_01.Models.Blog;
using MVC_01.Models.Contacts;
using MVC_01.Models.Product;

namespace MVC_01.Models
{
    // App.Models.AppDbContext
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
            modelBuilder.Entity<PostCategory>(entity =>
            {
                entity.HasKey(c => (new { c.PostID, c.CategoryID }));
            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
                   modelBuilder.Entity<ProductModel>(entity =>
            {
                entity.HasIndex(c => c.Slug).IsUnique();
            });
               modelBuilder.Entity<Product.ProductCategoryProduct>(entity =>
            {
                entity.HasKey(c => (new { c.ProductID, c.CategoryID }));
            });

        }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<ProductModel> Products {get;set;}
        public DbSet<ProductCategoryProduct>  ProductCategoryProducts{get;set;}
        public DbSet<CategoryProduct> CategoryProducts{get;set;}
        public DbSet<ProductPhoto> ProductPhotos {get;set;}
        public DbSet<PostPhoto> PostPhotos {get;set;}
    }
}

// dotnet aspnet-codegenerator controller -name CategoryController -namespace MVC_01.Areas.Blog.Controller -m MVC_01.Models.Blog.Category -dc MVC_01.Models.AppDbContext --relativeFolderPath Areas\Blog\Controller --useDefaultLayout