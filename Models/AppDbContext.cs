using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_01.Models.Contacts;

namespace MVC_01.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            //
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entity.SetTableName(tableName.Substring(6));
                }
            }
        }
        public DbSet<Contact> Contacts { get; set; }

    }
}

// dotnet aspnet-codegenerator controller -name Contact -namespace MVC_01.Areas.Contact.ContactController -m MVC_01.Models.Contacts.Contact -udl -dc MVC_01.Models.AppDbContext -outDir Areas/Contact/Controllers/
// dotnet aspnet-codegenerator controller -name ContactController -namespace MVC_01.Areas.Contact.ContactController -m MVC_01.Models.Contacts.Contact -dc MVC_01.Models.AppDbContext -outDir Areas\Contact\Controllers --useDefaultLayout