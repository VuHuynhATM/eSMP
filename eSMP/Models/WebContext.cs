using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Reflection.Metadata;

namespace eSMP.Models
{
    public class WebContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User_Address> User_Addresses { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Address> Addresss { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Store_Status> Store_Statuses { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Sub_Category> SubCategories { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=52.172.254.105,1433;Initial Catalog=eSMP;User ID=sa;Password =123456");
            }
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
            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Phone, e.RoleID })
                .IsUnique();
            modelBuilder.Entity<User>()
                .Property(e => e.Crete_date)
                .HasDefaultValueSql("getdate()");
            modelBuilder.Entity<User_Address>()
                .HasIndex(e => new { e.AddressID, e.UserID })
                .IsUnique();
            modelBuilder.Entity<Store>()
                .HasOne(e => e.User)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Store>()
                .HasIndex(e => new { e.UserID })
                .IsUnique();
        }
    }
}
