﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<Item_Status> Item_Statuses { get; set; }
        public DbSet<Specification> Specification { get; set; }
        public DbSet<Specification_Value> Specification_Values { get; set; }
        public DbSet<Sub_Item> Sub_Items { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Item_Image> Item_Images { get; set; }
        public DbSet<SubCate_Specification> SubCate_Specifications { get; set; }
        public DbSet<Brand>Brands { get; set; }
        public DbSet<Brand_Model> Brand_Models { get; set; }
        public DbSet<Model_Item> Model_Items { get; set; }
        public DbSet<SubItem_Status> subItem_Statuses { get; set; }


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
            modelBuilder.Entity<Specification_Value>()
                .HasOne(e => e.Specification)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Item_Image>()
                .HasOne(e => e.Item)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Sub_Item>()
                .HasOne(e => e.SubItem_Status)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
