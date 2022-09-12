using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace eSMP.Models
{
    public class WebContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Store_Img> Store_Imgs { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Sub_Category> SubCategories { get; set; }

        public const string ConnectStrring = @"Data Source=ec2-13-212-76-146.ap-southeast-1.compute.amazonaws.com,1433;Initial Catalog=eSMP;User ID=sa;Password = Sa123456";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectStrring);
            optionsBuilder.UseLoggerFactory(GetLoggerFactory());       // bật logger
        }

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
