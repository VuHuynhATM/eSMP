using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace eSMP.Models
{
    public class WebContext : DbContext
    {
        public WebContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Store_Img> Store_Imgs { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Sub_Category> SubCategories { get; set; }

        private ILoggerFactory GetLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                    builder.AddConsole()
                           .AddFilter(DbLoggerCategory.Database.Command.Name,
                                    LogLevel.Information));
            return serviceCollection.BuildServiceProvider()
                    .GetService<ILoggerFactory>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
        }
    }
}
