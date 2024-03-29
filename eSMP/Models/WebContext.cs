﻿using eSMP.VModels;
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
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Sub_Category> SubCategories { get; set; }
        public DbSet<Specification> Specification { get; set; }
        public DbSet<Specification_Value> Specification_Values { get; set; }
        public DbSet<Sub_Item> Sub_Items { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Item_Image> Item_Images { get; set; }
        public DbSet<SubCate_Specification> SubCate_Specifications { get; set; }
        public DbSet<Brand>Brands { get; set; }
        public DbSet<Brand_Model> Brand_Models { get; set; }
        public DbSet<Model_Item> Model_Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Feedback_Image> Feedback_Images { get; set; }  
        public DbSet<OrderBuy_Transacsion> orderBuy_Transacsions { get; set; }
        public DbSet<ShipStatus> ShipStatuses { get; set; }
        public DbSet<ShipOrder> ShipOrders { get; set; }
        public DbSet<OrderStore_Transaction> OrderStore_Transactions { get; set; }
        public DbSet<OrderSystem_Transaction> OrderSystem_Transactions { get; set; }
        public DbSet<eSMP_System> eSMP_Systems { get; set; }
        public DbSet<System_Withdrawal> System_Withdrawals { get; set; }
        public DbSet<Store_Withdrawal> Store_Withdrawals { get; set; }
        //public DbSet<BankSupport> BankSupports { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AddressVn> addressVns { get; set; }
        public DbSet<DataExchangeStore> DataExchangeStores { get; set; }
        public DbSet<DataExchangeUser> DataExchangeUsers { get; set; }
        public DbSet<ServiceDetail> ServiceDetails { get; set; }
        public DbSet<AfterBuyService> AfterBuyServices { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=SQL5107.site4now.net;Initial Catalog=db_a8ef86_esmp;User Id=db_a8ef86_esmp_admin;Password=Se1234567890");
                //optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=db_a8ef86_esmp;User Id=sa;Password=123456");
                optionsBuilder.UseLazyLoadingProxies();
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
            modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");
            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Phone, e.RoleID })
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.Phone })
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
            modelBuilder.Entity<Specification_Value>()
                .HasOne(e => e.Specification)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<System_Withdrawal>()
                .HasOne(e => e.eSMP_System)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<System_Withdrawal>()
                .HasOne(e => e.Image)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Item_Image>()
                .HasOne(e => e.Item)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Item_Image>()
               .HasOne(e => e.Image)
               .WithMany()
               .OnDelete(DeleteBehavior.Restrict);
            /* modelBuilder.Entity<Item_Image>()
                 .HasIndex(e => new { e.ImageID, e.ItemID })
                 .IsUnique();*/

            modelBuilder.Entity<Sub_Item>()
                .HasOne(e => e.Image)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(e => e.Sub_Item)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Feedback_Image>()
                .HasOne(e => e.OrderDetail)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrderStore_Transaction>()
                .HasOne(e => e.Store)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Report>()
                .HasOne(e => e.Store)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Report>()
                .HasOne(e => e.OrderDetail)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Report>()
                .HasOne(e => e.Item)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Model_Item>()
                .HasIndex(e => new { e.ItemID, e.Brand_ModelID})
                .IsUnique();

            modelBuilder.Entity<SubCate_Specification>()
                .HasOne(e => e.Specification)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<SubCate_Specification>()
                .HasOne(e => e.Sub_Category)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
           /* modelBuilder.Entity<SubCate_Specification>()
                .HasIndex(e => new { e.SpecificationID, e.Sub_CategoryID })
                .IsUnique();*/
            
            modelBuilder.Entity<Specification_Value>()
                .HasIndex(e => new { e.SpecificationID, e.ItemID })
                .IsUnique();
            modelBuilder.Entity<Feedback_Image>()
                .HasIndex(e => new { e.ImageID, e.OrderDetailID })
                .IsUnique();
            modelBuilder.Entity<OrderStore_Transaction>()
                .HasIndex(e => new { e.StoreID, e.OrderID })
                .IsUnique();
        }
    }
}
